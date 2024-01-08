using System.Collections.Concurrent;
using TrafficControlApp.Models;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Processors.ProcessorOrchestration;

public class ObjectPool<T>
{
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;

    public ObjectPool(Func<T> objectGenerator)
    {
        _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        _objects = new ConcurrentBag<T>();
    }

    public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

    public void Return(T item) => _objects.Add(item);
}

public class ProcessorPool<TItem>
{
    private IProcessor<TItem> RootProcessor;
    
    public void Foo()
    {
        // var pool = new ObjectPool<IProcessor<Track>>(() => );
    
        // pool.
    }
}