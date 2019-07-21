using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Titanium.Web.Proxy.Helpers
{
    internal class TaskHelper
    {
        public static Task Delay(int mills, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() =>
            {
                var start = DateTime.Now.Ticks / 1000;
                var exp = start + mills;
                var sl = Math.Min(mills, 3000);
                if(sl == mills)
                {
                    Thread.Sleep(mills);
                }
                else
                {
                    while (true)
                    {
                        if (start + sl >= exp)
                        {
                            Thread.Sleep((int)(exp - start));
                            break;
                        }
                        else
                        {
                            start += sl;
                            Thread.Sleep(sl);
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
            });
        }

        public static Task<Task> WhenAny(params Task[] tasks)
        {
            return Task.Factory.StartNew(() =>
            {
                int i = Task.WaitAny(tasks);
                return tasks[i];
            });
        }

        public static Task WhenAll(params Task[] tasks)
        {
            return Task.Factory.StartNew(() =>
            {
                Task.WaitAll(tasks);
            });
        }

        public static Task<T> FromResult<T>(T result)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            taskCompletionSource.SetResult(result);
            return taskCompletionSource.Task;
        }
    }
}
