using System;
using System.Text;
using Keyence.HostLink.Models;

namespace Keyence.HostLink.Protocol
{
    internal static class CommandBuilder
    {
        private const string CMD_RD = "RD ";
        private const string CMD_WR = "WR ";
        private const string CMD_RCS = "RCS ";
        private const string CMD_WCS = "WCS ";
        private const string LINE_END = "\r\n";

        public static byte[] BuildReadCommand(string address)
        {
            var command = CMD_RD + address + LINE_END;
            return Encoding.ASCII.GetBytes(command);
        }

        public static byte[] BuildWriteCommand(string address, string value)
        {
            var command = CMD_WR + address + " " + value + LINE_END;
            return Encoding.ASCII.GetBytes(command);
        }

        public static byte[] BuildReadContinuousCommand(int startAddress, int count)
        {
            var command = CMD_RCS + startAddress.ToString("D4") + count.ToString("D4") + LINE_END;
            return Encoding.ASCII.GetBytes(command);
        }

        public static byte[] BuildWriteContinuousCommand(int startAddress, int count, short[] values)
        {
            var sb = new StringBuilder();
            sb.Append(CMD_WCS);
            sb.Append(startAddress.ToString("D4"));
            sb.Append(count.ToString("D4"));
            sb.Append(" ");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0) sb.Append(" ");
                sb.Append(values[i].ToString());
            }
            sb.Append(LINE_END);
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        public static byte[] BuildReadCommand(SoftElementType elementType, int address)
        {
            var addressStr = elementType.ToString() + address;
            return BuildReadCommand(addressStr);
        }

        public static byte[] BuildWriteCommand(SoftElementType elementType, int address, string value)
        {
            var addressStr = elementType.ToString() + address;
            return BuildWriteCommand(addressStr, value);
        }
    }
}
