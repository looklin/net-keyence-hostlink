using System;

namespace Keyence.HostLink.Exceptions
{
    public class ProtocolException : HostLinkException
    {
        public string? RawResponse { get; }

        public ProtocolException(string message, string? rawResponse = null) 
            : base(message) 
        {
            RawResponse = rawResponse;
        }

        public ProtocolException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
