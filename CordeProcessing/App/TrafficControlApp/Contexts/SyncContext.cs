using System.Collections.Concurrent;
using TrafficControlApp.Models;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Contexts;

public class SyncContext<TInput>
{
    public SemaphoreSlim SemaphoreSlim { get; set; } = new (1, 1);
    public ConcurrentBag<object> SyncList { get; set; }
    public int count;

    public int Count
    {
        get
        {
            return count;
        }
    }

    public SyncContext()
    {
        SyncList = new ();
    }

    public async Task<bool> WaitLock(IProcessor<TInput> processor)
    {
        await SemaphoreSlim.WaitAsync();
        if (processor.ProcessorsExecuting.TryPop(out processor))
        {
            SemaphoreSlim.Release();
            return true;
        }
        count++;

        return false;
    }

    public void ReleaseLock(object obj)
    { 
        SemaphoreSlim.Release();
        Interlocked.Increment(ref count);
    }
}