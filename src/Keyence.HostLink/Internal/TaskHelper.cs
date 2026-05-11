using System;
using System.Threading.Tasks;

namespace Keyence.HostLink.Internal
{
    internal static class TaskHelper
    {
        public static Task<T> FromResult<T>(T result)
        {
            return new TaskCompletionSource<T>(result).Task;
        }

        public static Task CompletedTask()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }

        public static Task<T> FromException<T>(Exception exception)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
