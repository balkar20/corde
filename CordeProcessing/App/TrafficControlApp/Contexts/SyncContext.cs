using System.Collections.Concurrent;

namespace TrafficControlApp.Contexts;

public class SyncContext
{
    public SemaphoreSlim SemaphoreSlim { get; set; } = new (1, 1);
    public ConcurrentBag<object> SyncList { get; set; }
    public int  count { get; set; }
    public SyncContext()
    {
        SyncList = new ();
    }

    public async Task WaitLock()
    {
        await SemaphoreSlim.WaitAsync();
        count++;
        
    }

    public void ReleaseLock(object obj)
    { 
        SemaphoreSlim.Release();
        count--;
    }
}