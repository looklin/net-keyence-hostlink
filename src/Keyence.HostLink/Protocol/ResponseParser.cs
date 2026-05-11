using System;
using System.Text;

namespace Keyence.HostLink.Protocol
{
    internal static class ResponseParser
    {
        public static string ParseResponse(byte[] buffer, int bytesRead)
        {
            if (buffer == null || bytesRead <= 0)
                return string.Empty;

            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }

        public static bool IsErrorResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return true;

            var trimmed = response.Trim('\r', '\n', ' ');

            if (trimmed.StartsWith("?") || trimmed.StartsWith("E"))
                return true;

            return false;
        }

        public static string GetErrorMessage(string response)
        {
            if (string.IsNullOrEmpty(response))
                return "Unknown error";

            var trimmed = response.Trim('\r', '\n', ' ');

            if (trimmed.StartsWith("?"))
            {
                return ParseErrorCode(trimmed);
            }

            return trimmed;
        }

        private static string ParseErrorCode(string response)
        {
            var code = response.Trim('?', '\r', '\n', ' ');
            switch (code)
            {
                case "01":
                    return "Command format error";
                case "02":
                    return "Command error";
                case "03":
                    return "Address error";
                case "04":
                    return "Data range error";
                case "10":
                    return "Write not allowed";
                default:
                    return "Error code: " + code;
            }
        }

        public static byte[] ExtractDataBytes(string response)
        {
            if (string.IsNullOrEmpty(response))
                return new byte[0];

            var trimmed = response.Trim('\r', '\n', ' ');

            if (IsErrorResponse(trimmed))
                return new byte[0];

            return Encoding.ASCII.GetBytes(trimmed);
        }
    }
}
