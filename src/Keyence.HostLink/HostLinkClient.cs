using System;
using System.Threading;
using System.Threading.Tasks;
using Keyence.HostLink.Internal;
using Keyence.HostLink.Models;
using Keyence.HostLink.Protocol;
using Keyence.HostLink.Transports;

namespace Keyence.HostLink
{
    public class HostLinkClient : IDisposable
    {
        private readonly HostLinkOptions _options;
        private readonly ITransport _transport;
        private readonly CommandQueue _commandQueue;
        private readonly object _connectLock = new object();
        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        private volatile bool _disposed;
        private Timer _heartbeatTimer;
        private int _reconnectAttempts;

        public event EventHandler<ConnectionEventArgs> Connected;
        public event EventHandler<ConnectionEventArgs> Disconnected;

        public ConnectionStatus Status
        {
            get { return _status; }
            private set
            {
                var oldStatus = _status;
                _status = value;
                if (oldStatus != value)
                {
                    OnStatusChanged(value);
                }
            }
        }

        public HostLinkOptions Options
        {
            get { return _options; }
        }

        public bool IsConnected
        {
            get { return _transport.IsConnected && Status == ConnectionStatus.Connected; }
        }

        public HostLinkClient(HostLinkOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            _options = options;
            _transport = new TcpTransport(_options.Host, _options.Port,
                (int)_options.ConnectTimeout.TotalMilliseconds, _options.ReceiveBufferSize);
            _commandQueue = new CommandQueue(_transport);

            _transport.Error += OnTransportError;
        }

        public async Task ConnectAsync()
        {
            await ConnectAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (IsConnected)
                return;

            lock (_connectLock)
            {
                if (IsConnected)
                    return;
            }

            Status = ConnectionStatus.Connecting;
            _reconnectAttempts = 0;

            try
            {
                await _transport.ConnectAsync().ConfigureAwait(false);
                Status = ConnectionStatus.Connected;
                OnConnected();

                if (_options.EnableHeartbeat)
                {
                    StartHeartbeat();
                }
            }
            catch (Exception ex)
            {
                Status = ConnectionStatus.Error;
                throw new Exceptions.ConnectionException("Failed to connect to PLC.", ex);
            }
        }

        public void Disconnect()
        {
            if (_disposed)
                return;

            StopHeartbeat();
            Status = ConnectionStatus.Disconnected;

            try
            {
                _transport.DisconnectAsync().Wait();
            }
            catch
            {
            }

            OnDisconnected();
        }

        public async Task<HostLinkResult> ReadItemAsync(string address)
        {
            return await ReadItemAsync(address, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<HostLinkResult> ReadItemAsync(string address, CancellationToken cancellationToken)
        {
            EnsureConnected();

            var command = CommandBuilder.BuildReadCommand(address);
            var timeoutMs = (int)_options.ReadTimeout.TotalMilliseconds;

            var result = await _commandQueue.EnqueueAsync(command, address, timeoutMs).ConfigureAwait(false);

            if (!result.IsSuccess && _options.AutoReconnect)
            {
                await HandleCommandFailureAsync().ConfigureAwait(false);
            }

            return result;
        }

        public Task<HostLinkResult> ReadItemAsync(int address, SoftElementType type)
        {
            return ReadItemAsync(address, type, CancellationToken.None);
        }

        public async Task<HostLinkResult> ReadItemAsync(int address, SoftElementType type, CancellationToken cancellationToken)
        {
            var addressStr = type.ToString() + address.ToString();
            return await ReadItemAsync(addressStr, cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteItemAsync(string address, string value)
        {
            await WriteItemAsync(address, value, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task WriteItemAsync(string address, string value, CancellationToken cancellationToken)
        {
            EnsureConnected();

            var command = CommandBuilder.BuildWriteCommand(address, value);
            var timeoutMs = (int)_options.WriteTimeout.TotalMilliseconds;

            var result = await _commandQueue.EnqueueAsync(command, address, timeoutMs).ConfigureAwait(false);

            if (!result.IsSuccess && _options.AutoReconnect)
            {
                await HandleCommandFailureAsync().ConfigureAwait(false);
            }

            if (!result.IsSuccess)
            {
                throw new Exceptions.ProtocolException(
                    "Failed to write to " + address + ": " + ResponseParser.GetErrorMessage(result.RawResponse),
                    result.Error);
            }
        }

        public Task WriteItemAsync(string address, short value)
        {
            return WriteItemAsync(address, value.ToString());
        }

        public Task WriteItemAsync(int address, SoftElementType type, short value)
        {
            var addressStr = type.ToString() + address.ToString();
            return WriteItemAsync(addressStr, value.ToString());
        }

        public async Task<HostLinkResult> ReadContinuousAsync(string startAddress, int count)
        {
            return await ReadContinuousAsync(startAddress, count, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task<HostLinkResult> ReadContinuousAsync(string startAddress, int count, CancellationToken cancellationToken)
        {
            EnsureConnected();

            var addressNum = AddressParser.ParseElementAddress(startAddress);
            var command = CommandBuilder.BuildReadContinuousCommand(addressNum, count);
            var timeoutMs = (int)_options.ReadTimeout.TotalMilliseconds;

            var result = await _commandQueue.EnqueueAsync(command, startAddress, timeoutMs).ConfigureAwait(false);

            if (!result.IsSuccess && _options.AutoReconnect)
            {
                await HandleCommandFailureAsync().ConfigureAwait(false);
            }

            return result;
        }

        public async Task WriteContinuousAsync(string startAddress, short[] values)
        {
            EnsureConnected();

            var addressNum = AddressParser.ParseElementAddress(startAddress);
            var command = CommandBuilder.BuildWriteContinuousCommand(addressNum, values.Length, values);
            var timeoutMs = (int)_options.WriteTimeout.TotalMilliseconds;

            var result = await _commandQueue.EnqueueAsync(command, startAddress, timeoutMs).ConfigureAwait(false);

            if (!result.IsSuccess && _options.AutoReconnect)
            {
                await HandleCommandFailureAsync().ConfigureAwait(false);
            }

            if (!result.IsSuccess)
            {
                throw new Exceptions.ProtocolException(
                    "Failed to write continuous to " + startAddress + ": " + ResponseParser.GetErrorMessage(result.RawResponse),
                    result.Error);
            }
        }

        private void EnsureConnected()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!IsConnected)
            {
                throw new Exceptions.ConnectionException("Not connected to PLC. Call ConnectAsync() first.");
            }
        }

        private async Task HandleCommandFailureAsync()
        {
            if (!_options.AutoReconnect)
                return;

            if (_options.MaxReconnectAttempts >= 0 && _reconnectAttempts >= _options.MaxReconnectAttempts)
                return;

            Status = ConnectionStatus.Reconnecting;
            _reconnectAttempts++;

            try
            {
                await _transport.DisconnectAsync().ConfigureAwait(false);
            }
            catch
            {
            }

            await Task.Delay(_options.ReconnectInterval).ConfigureAwait(false);

            try
            {
                await _transport.ConnectAsync().ConfigureAwait(false);
                Status = ConnectionStatus.Connected;
                _reconnectAttempts = 0;
                OnConnected();
            }
            catch
            {
                Status = ConnectionStatus.Error;
                OnDisconnected();
            }
        }

        private void StartHeartbeat()
        {
            StopHeartbeat();
            var intervalMs = (int)_options.HeartbeatInterval.TotalMilliseconds;
            _heartbeatTimer = new Timer(HeartbeatCallback, null, intervalMs, intervalMs);
        }

        private void StopHeartbeat()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Dispose();
                _heartbeatTimer = null;
            }
        }

        private async void HeartbeatCallback(object state)
        {
            if (!IsConnected)
                return;

            try
            {
                var command = CommandBuilder.BuildReadCommand("DM0");
                var timeoutMs = (int)_options.ReadTimeout.TotalMilliseconds;
                await _commandQueue.EnqueueAsync(command, "DM0", timeoutMs).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        private void OnTransportError(object sender, TransportErrorEventArgs e)
        {
            Status = ConnectionStatus.Error;
            OnDisconnected();
        }

        private void OnStatusChanged(ConnectionStatus newStatus)
        {
        }

        private void OnConnected()
        {
            var handler = Connected;
            if (handler != null)
            {
                try
                {
                    handler(this, new ConnectionEventArgs(ConnectionStatus.Connected));
                }
                catch
                {
                }
            }
        }

        private void OnDisconnected()
        {
            var handler = Disconnected;
            if (handler != null)
            {
                try
                {
                    handler(this, new ConnectionEventArgs(ConnectionStatus.Disconnected));
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Disconnect();
            _transport.Dispose();
            StopHeartbeat();
        }
    }
}
