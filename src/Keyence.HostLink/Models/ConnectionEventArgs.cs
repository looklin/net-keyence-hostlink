using System;

namespace Keyence.HostLink.Models
{
    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionStatus Status { get; }
        public Exception? Error { get; }
        public string Message { get; }

        public ConnectionEventArgs(ConnectionStatus status, Exception? error = null, string? message = null)
        {
            Status = status;
            Error = error;
            Message = message ?? string.Empty;
        }
    }
}
