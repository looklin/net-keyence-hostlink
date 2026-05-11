using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Keyence.HostLink.Models;
using Keyence.HostLink.Transports;

namespace Keyence.HostLink.Serial
{
    public class SerialTransport : ITransport
    {
        private readonly SerialPort _serialPort;
        private bool _disposed;

        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public event EventHandler<TransportErrorEventArgs> Error;

        public SerialTransport(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        public async Task ConnectAsync()
        {
            await Task.Run(() => _serialPort.Open()).ConfigureAwait(false);
        }

        public async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }).ConfigureAwait(false);
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds)
        {
            _serialPort.ReadTimeout = timeoutMilliseconds;
            return await Task.Run(() => _serialPort.Read(buffer, offset, count)).ConfigureAwait(false);
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds)
        {
            _serialPort.WriteTimeout = timeoutMilliseconds;
            await Task.Run(() => _serialPort.Write(buffer, offset, count)).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                    _serialPort.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}
