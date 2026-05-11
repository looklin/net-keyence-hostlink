using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Keyence.HostLink.Exceptions;

namespace Keyence.HostLink.Transports
{
    public class TcpTransport : ITransport
    {
        private readonly string _host;
        private readonly int _port;
        private readonly int _connectTimeoutMs;
        private readonly int _receiveBufferSize;

        private TcpClient _client;
        private NetworkStream _stream;
        private volatile bool _connected;
        private bool _disposed;
        private readonly object _disconnectLock = new object();

        public bool IsConnected => _connected;

        public event EventHandler<TransportErrorEventArgs> Error;

        public TcpTransport(string host, int port, int connectTimeoutMs = 5000, int receiveBufferSize = 4096)
        {
            _host = host;
            _port = port;
            _connectTimeoutMs = connectTimeoutMs;
            _receiveBufferSize = receiveBufferSize;
        }

        public async Task ConnectAsync()
        {
            if (_connected)
                return;

            try
            {
                _client = new TcpClient();
                _client.ReceiveBufferSize = _receiveBufferSize;
                _client.SendBufferSize = _receiveBufferSize;

                var connectTask = _client.ConnectAsync(_host, _port);
                var timeoutTask = Task.Delay(_connectTimeoutMs);

                if (await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false) == timeoutTask)
                {
                    try { _client.Close(); } catch { }
                    throw new Exceptions.TimeoutException("Connection to " + _host + ":" + _port + " timed out after " + _connectTimeoutMs + "ms.");
                }

                await connectTask.ConfigureAwait(false);
                _stream = _client.GetStream();
                _stream.ReadTimeout = _connectTimeoutMs;
                _stream.WriteTimeout = _connectTimeoutMs;
                _connected = true;
            }
            catch (ConnectionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _connected = false;
                throw new ConnectionException("Failed to connect to " + _host + ":" + _port + ".", ex);
            }
        }

        public async Task DisconnectAsync()
        {
            lock (_disconnectLock)
            {
                if (!_connected)
                    return;
            }

            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                _connected = false;
            }
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds)
        {
            if (!_connected || _stream == null)
                throw new ConnectionException("Not connected.");

            try
            {
                if (_stream.CanTimeout)
                {
                    _stream.ReadTimeout = timeoutMilliseconds;
                }

                var readTask = _stream.ReadAsync(buffer, offset, count);
                var timeoutTask = Task.Delay(timeoutMilliseconds);

                if (await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(false) == timeoutTask)
                {
                    throw new Exceptions.TimeoutException("Read timed out after " + timeoutMilliseconds + "ms.");
                }

                var bytesRead = await readTask.ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    _connected = false;
                    throw new ConnectionException("Remote host closed the connection.");
                }
                return bytesRead;
            }
            catch (Exceptions.TimeoutException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                _connected = false;
                throw new ConnectionException("Connection was closed.");
            }
            catch (IOException)
            {
                _connected = false;
                throw;
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw new ConnectionException("Read operation failed.", ex);
            }
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds)
        {
            if (!_connected || _stream == null)
                throw new ConnectionException("Not connected.");

            try
            {
                if (_stream.CanTimeout)
                {
                    _stream.WriteTimeout = timeoutMilliseconds;
                }

                var writeTask = _stream.WriteAsync(buffer, offset, count);
                var timeoutTask = Task.Delay(timeoutMilliseconds);

                if (await Task.WhenAny(writeTask, timeoutTask).ConfigureAwait(false) == timeoutTask)
                {
                    throw new Exceptions.TimeoutException("Write timed out after " + timeoutMilliseconds + "ms.");
                }

                await writeTask.ConfigureAwait(false);
            }
            catch (Exceptions.TimeoutException)
            {
                throw;
            }
            catch (ObjectDisposedException)
            {
                _connected = false;
                throw new ConnectionException("Connection was closed.");
            }
            catch (IOException)
            {
                _connected = false;
                throw;
            }
            catch (Exception ex)
            {
                OnError(ex);
                throw new ConnectionException("Write operation failed.", ex);
            }
        }

        protected virtual void OnError(Exception ex)
        {
            var handler = Error;
            if (handler != null)
            {
                handler(this, new TransportErrorEventArgs(ex));
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            try
            {
                DisconnectAsync().Wait();
            }
            catch
            {
            }
        }
    }
}
