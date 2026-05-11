using System;

namespace Keyence.HostLink.Exceptions
{
    public class HostLinkException : Exception
    {
        public HostLinkException(string message) : base(message) { }
        public HostLinkException(string message, Exception innerException) : base(message, innerException) { }
    }
}
