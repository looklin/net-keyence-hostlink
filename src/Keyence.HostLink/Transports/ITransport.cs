using System;
using System.Threading.Tasks;

namespace Keyence.HostLink.Transports
{
    public interface ITransport : IDisposable
    {
        bool IsConnected { get; }

        Task ConnectAsync();

        Task DisconnectAsync();

        Task<int> ReadAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds);

        Task WriteAsync(byte[] buffer, int offset, int count, int timeoutMilliseconds);

        event EventHandler<TransportErrorEventArgs> Error;
    }

    public class TransportErrorEventArgs : EventArgs
    {
        public Exception Error { get; }

        public TransportErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
