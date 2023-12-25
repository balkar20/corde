using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors;
using DataFlowProducerConsumer.Processors.Abstractions;
using DataFlowProducerConsumer.PrsStructure;

namespace DataFlowProducerConsumer;

public class ProducerConsumerArena2
{
    public int AmountOfThreads { get; set; }
    public SortedSet<int> Treads { get; set; }
    private static TimeSpan produceSpeed = TimeSpan.FromSeconds(0.5);
    const int produceCount = 20;
    private readonly ITrackDevice _trackDevice;
    private readonly IProcessor<Track, VehicleTypeProcessResult> _processor;
    private readonly Processor<Track, PoolProcessResult> _processorPool;
    
    // private readonly IProcessorPoolStateComposite _processorPoolStateComposite;

    // public ProducerConsumerArena2(ITrackDevice trackDevice)
    // {
    //     _trackDevice = trackDevice;
    //     Treads = new SortedSet<int>();
    //     _processorPool = new ProcessorPool<PoolProcessResult>();
    //
    // }
    
    
    
    private static  TimeSpan consumeSpeed = TimeSpan.FromSeconds(2);

    public ProducerConsumerArena2(ITrackDevice trackDevice, Processor<Track, PoolProcessResult> processorPool)
    {
        _processorPool = processorPool;
    }


    // public async Task Demonstrate()
    // {
    //     // var buffer = new BufferBlock<string>();
    //     var buffer = new BufferBlock<string>();
    //     var consumerTask = ConsumeAllAsync(buffer);
    //     var producerTask = ProduceAsync(buffer);
    //     await Task.WhenAll(consumerTask, producerTask);
    //     var itemsProcessed = consumerTask.Result;
    // }

    // [Benchmark]
    public async Task DemonstrateSec()
    {
        var maxParallelConsume = 9;
        
        var buffer = new BufferBlock<Track>(
            new DataflowBlockOptions() { BoundedCapacity = maxParallelConsume });
        var consumerBlock = new ActionBlock<Track>(
            track => ConsumeOneTrackAsync(track),
            new ExecutionDataflowBlockOptions 
            { BoundedCapacity = maxParallelConsume,
                MaxDegreeOfParallelism = maxParallelConsume });
        buffer.LinkTo(consumerBlock, new DataflowLinkOptions() 
            { PropagateCompletion = true });

        var producerTask = ProduceAsync(buffer);
        await consumerBlock.Completion;
    }
    
    async Task ProduceAsync(ITargetBlock<Track> target)
    {
        var sourceData = await _trackDevice.GiveMeTrackDataBunch("mark");
        foreach (var track in sourceData.Tracks)
        {
            var item = $"Item {track.TrackId + 1}";
            Console.WriteLine($"Producing {item}, ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
            await Task.Delay(produceSpeed);
            // Treads.Add(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine($"Produced {item}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");

            await target.SendAsync(track);
        }

        target.Complete();
    }

    async Task ConsumeOneAsync(string message)
    {
        
        Console.WriteLine($"Consuming {message}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
        //Here We need to use Processor
        await Task.Delay(consumeSpeed);
        Treads.Add(Thread.CurrentThread.ManagedThreadId);
        // PrintThreadInfo();
        Treads.Add(Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine($"Consumed {message}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
        Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");
    }

    async Task ConsumeOneTrackAsync(Track track)
    {
        Console.WriteLine($"Consuming track {track.TrackId}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
        //Here We need to use Processor
        // await Task.Delay(consumeSpeed);
        // var processResult = await _processor.ProcessAsync(track);
        var processResult = await _processorPool.ProcessAsync(track);
        Treads.Add(Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine($"Consumed {track.TrackId} with processed result = {processResult} ? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
        Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");
        
        LinkedList<Processor<>> people = new LinkedList<string>(new[] { "Tom", "Sam", "Bob" });
    }
    
    async Task<int> ConsumeAllAsync(ISourceBlock<string> source)
    {
        int itemsProcessed = 0;

        while (await source.OutputAvailableAsync())
        {
            var data = await source.ReceiveAsync();
            await ConsumeOneAsync(data);
            itemsProcessed++;
        }

        return itemsProcessed;
    }

    // private Task SaveValToBuffer(int val)
    // {
    //     Treads.Add()
    // }

    private void PrintThreadInfo()
    {
        // Get the number of threads currently in use by the thread pool.
        ThreadPool.GetAvailableThreads(out int workerThreads, out int cpThreads);

// Print the number of threads to the console.
        Console.WriteLine("There are {0} threads currently in use by the thread pool.", workerThreads);
    }
}