using System.Threading;
using System.Threading.Tasks;

namespace Algo.Common
{
    public static class WaitHandleExtensions
    {
        public static async Task<bool> WaitOneAsync(this WaitHandle waitHandle, int timeoutMs = Timeout.Infinite)
        {
            void WaitHandleCallback(object state, bool timedOut)
            {
                var taskCompletionSource = (TaskCompletionSource<bool>) state;
                taskCompletionSource.SetResult(!timedOut);
            }

            var tcs = new TaskCompletionSource<bool>();
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle, WaitHandleCallback, tcs, timeoutMs, true);

            await tcs.Task;
            rwh.Unregister(null);
            return tcs.Task.Result;
        }
    }
}
