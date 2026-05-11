using System;
using System.Buffers;
using System.Threading.Tasks;

namespace Keyence.HostLink.Net6
{
    public static class ValueTaskExtensions
    {
        public static ValueTask<int> ReadAsyncValueTask(
            System.IO.Stream stream,
            Memory<byte> buffer,
            int timeoutMilliseconds)
        {
            if (stream.CanTimeout)
            {
                stream.ReadTimeout = timeoutMilliseconds;
            }

            return stream.ReadAsync(buffer);
        }

        public static ValueTask WriteAsyncValueTask(
            System.IO.Stream stream,
            ReadOnlyMemory<byte> buffer,
            int timeoutMilliseconds)
        {
            if (stream.CanTimeout)
            {
                stream.WriteTimeout = timeoutMilliseconds;
            }

            return stream.WriteAsync(buffer);
        }
    }
}
