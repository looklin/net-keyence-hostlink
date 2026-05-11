using System;

namespace Keyence.HostLink
{
    public class HostLinkOptions
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 8501;

        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromSeconds(3);

        public TimeSpan WriteTimeout { get; set; } = TimeSpan.FromSeconds(3);

        public bool AutoReconnect { get; set; } = true;

        public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(5);

        public int MaxReconnectAttempts { get; set; } = -1;

        public bool EnableHeartbeat { get; set; } = false;

        public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(30);

        public byte StationNumber { get; set; } = 0;

        public int ReceiveBufferSize { get; set; } = 4096;
    }
}
