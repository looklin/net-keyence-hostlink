using System;
using Keyence.HostLink.Models;

namespace Keyence.HostLink.Protocol
{
    internal static class AddressParser
    {
        private static readonly string[] ValidPrefixes = new string[] { "DM", "LR", "MR", "TC", "CC", "EM", "R", "L", "M", "T", "C" };

        public static SoftElementType ParseElementType(string address)
        {
            if (string.IsNullOrEmpty(address))
                return SoftElementType.Unknown;

            return ParseAddress(address, out _, out _);
        }

        public static int ParseElementAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address cannot be null or empty.", "address");

            var elementType = ParseAddress(address, out int elementAddress, out bool success);
            if (!success)
                throw new ArgumentException("Invalid address format: " + address, "address");

            return elementAddress;
        }

        public static bool TryParse(string address, out SoftElementType elementType, out int elementAddress)
        {
            elementType = SoftElementType.Unknown;
            elementAddress = -1;

            if (string.IsNullOrEmpty(address))
                return false;

            elementType = ParseAddress(address, out elementAddress, out bool success);
            return success;
        }

        private static SoftElementType ParseAddress(string address, out int elementAddress, out bool success)
        {
            elementAddress = 0;
            success = false;

            var prefix = GetPrefix(address);
            if (prefix == null)
                return SoftElementType.Unknown;

            var numberPart = address.Substring(prefix.Length);
            if (!int.TryParse(numberPart, out elementAddress))
                return SoftElementType.Unknown;

            success = true;
            return PrefixToElementType(prefix);
        }

        private static string GetPrefix(string address)
        {
            if (address.Length < 2)
                return null;

            for (int i = 0; i < ValidPrefixes.Length; i++)
            {
                var prefix = ValidPrefixes[i];
                if (address.Length >= prefix.Length && address.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var remaining = address.Substring(prefix.Length);
                    if (remaining.Length > 0 && char.IsDigit(remaining[0]))
                        return prefix;
                }
            }
            return null;
        }

        private static SoftElementType PrefixToElementType(string prefix)
        {
            var upper = prefix.ToUpperInvariant();
            switch (upper)
            {
                case "DM": return SoftElementType.DM;
                case "R": return SoftElementType.R;
                case "LR": return SoftElementType.LR;
                case "MR": return SoftElementType.MR;
                case "T": return SoftElementType.T;
                case "C": return SoftElementType.C;
                case "TC": return SoftElementType.TC;
                case "CC": return SoftElementType.CC;
                case "EM": return SoftElementType.EM;
                default: return SoftElementType.Unknown;
            }
        }
    }
}
