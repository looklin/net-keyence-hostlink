using System;
using System.Text;

namespace Keyence.HostLink.Models
{
    public class HostLinkResult
    {
        public byte[] RequestMessage { get; }
        public string Address { get; }
        public string RawResponse { get; }
        public byte[] ResponseBuffer { get; }
        public bool IsSuccess { get; }
        public Exception? Error { get; }

        internal HostLinkResult(
            byte[] requestMessage,
            string address,
            string rawResponse,
            byte[] responseBuffer,
            bool isSuccess,
            Exception? error = null)
        {
            RequestMessage = requestMessage ?? Array.Empty<byte>();
            Address = address ?? string.Empty;
            RawResponse = rawResponse ?? string.Empty;
            ResponseBuffer = responseBuffer ?? Array.Empty<byte>();
            IsSuccess = isSuccess;
            Error = error;
        }

        internal static HostLinkResult Fail(byte[] requestMessage, string address, Exception error)
        {
            return new HostLinkResult(requestMessage, address, string.Empty, Array.Empty<byte>(), false, error);
        }

        internal static HostLinkResult Success(byte[] requestMessage, string address, string rawResponse, byte[] responseBuffer)
        {
            return new HostLinkResult(requestMessage, address, rawResponse, responseBuffer, true);
        }

        public short ToInt16()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            if (short.TryParse(cleanValue, out var result))
                return result;

            if (cleanValue.StartsWith("#") && int.TryParse(cleanValue.Substring(1), out var decResult))
                return (short)decResult;

            throw new FormatException($"Cannot convert '{RawResponse}' to Int16.");
        }

        public int ToInt32()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            if (int.TryParse(cleanValue, out var result))
                return result;

            if (cleanValue.StartsWith("#") && int.TryParse(cleanValue.Substring(1), out var decResult))
                return decResult;

            throw new FormatException($"Cannot convert '{RawResponse}' to Int32.");
        }

        public ushort ToUInt16()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            if (ushort.TryParse(cleanValue, out var result))
                return result;

            throw new FormatException($"Cannot convert '{RawResponse}' to UInt16.");
        }

        public uint ToUInt32()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            if (uint.TryParse(cleanValue, out var result))
                return result;

            throw new FormatException($"Cannot convert '{RawResponse}' to UInt32.");
        }

        public float ToSingle()
        {
            if (!IsSuccess || ResponseBuffer == null || ResponseBuffer.Length < 4)
                throw new InvalidOperationException("Result is not valid for conversion.");

            return BitConverter.ToSingle(ResponseBuffer, 0);
        }

        public bool ToBoolean()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            return cleanValue == "1" || cleanValue.ToUpperInvariant() == "ON" || cleanValue.ToUpperInvariant() == "TRUE";
        }

        public short[] ToIntArray()
        {
            if (!IsSuccess || string.IsNullOrEmpty(RawResponse))
                throw new InvalidOperationException("Result is not valid for conversion.");

            var cleanValue = CleanResponse(RawResponse);
            var parts = cleanValue.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new short[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                result[i] = short.Parse(parts[i]);
            }
            return result;
        }

        private static string CleanResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return string.Empty;

            return response.Trim('\r', '\n', ' ', '\0');
        }
    }
}
