using System;

namespace Keyence.HostLink.Exceptions
{
    public class TimeoutException : HostLinkException
    {
        public TimeoutException(string message) : base(message) { }
        public TimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }
}
