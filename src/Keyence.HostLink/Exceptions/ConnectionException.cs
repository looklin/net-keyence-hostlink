using System;

namespace Keyence.HostLink.Exceptions
{
    public class ConnectionException : HostLinkException
    {
        public ConnectionException(string message) : base(message) { }
        public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
