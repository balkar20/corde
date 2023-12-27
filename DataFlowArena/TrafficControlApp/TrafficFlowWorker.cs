using System.Threading.Tasks.Dataflow;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.PrsStructure;
using TrafficControlApp.Producers.Abstraction;

namespace TrafficControlApp;

public class TrafficFlowWorker
{
    public SortedSet<int> Treads { get; set; }
    private readonly ITrackDevice _trackDevice;
    // private readonly IProcessor<Track, VehicleTypeProcessResult> _processor;
    private readonly Processor<Track, IAnalysingResult> _processorPool;
    private readonly ITrackProducer _trackProducer;
    private readonly ITrackConsumer _trackConsumer;
    
    
    
    // private readonly IProcessorPoolStateComposite _processorPoolStateComposite;

    // public ProducerConsumerArena2(ITrackDevice trackDevice)
    // {
    //     _trackDevice = trackDevice;
    //     Treads = new SortedSet<int>();
    //     _processorPool = new ProcessorPool<PoolProcessResult>();
    //
    // }
    
    private static  TimeSpan consumeSpeed = TimeSpan.FromSeconds(2);

    public TrafficFlowWorker(ITrackDevice trackDevice, Processor<Track, IAnalysingResult> processorPool, ITrackProducer trackProducer, ITrackConsumer trackConsumer)
    {
        _processorPool = processorPool;
        _trackProducer = trackProducer;
        _trackConsumer = trackConsumer;
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
    public async Task StartProcess()
    {
        var maxParallelConsume = 9;
        
        var buffer = new BufferBlock<Track>(
            new DataflowBlockOptions() { BoundedCapacity = maxParallelConsume });
        
        await _trackConsumer.ConsumeAllAsync(buffer, block => 
            _trackProducer.ProduceAllAsync(block));
        // var consumerBlock = new ActionBlock<Track>(
        //     track => ConsumeOneTrackAsync(track),
        //     new ExecutionDataflowBlockOptions 
        //     { BoundedCapacity = maxParallelConsume,
        //         MaxDegreeOfParallelism = maxParallelConsume });
        // buffer.LinkTo(consumerBlock, new DataflowLinkOptions() 
        //     { PropagateCompletion = true });

        // var producerTask = ProduceAsync(buffer);
        // var producerTask = _trackProducer.ProduceAllAsync(buffer);
        // await consumerBlock.Completion;
    }
    
    // async Task ProduceAsync(ITargetBlock<Track> target)
    // {
    //     var sourceData = await _trackDevice.GiveMeTrackDataBunch("mark");
    //     foreach (var track in sourceData.Tracks)
    //     {
    //         var item = $"Item {track.TrackId + 1}";
    //         Console.WriteLine($"Producing {item}, ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
    //         await Task.Delay(produceSpeed);
    //         // Treads.Add(Thread.CurrentThread.ManagedThreadId);
    //         Console.WriteLine($"Produced {item}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
    //         Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");
    //
    //         await target.SendAsync(track);
    //     }
    //
    //     target.Complete();
    // }

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
        await _processorPool.ProcessNextAsync(track);
        Treads.Add(Thread.CurrentThread.ManagedThreadId);
        // Console.WriteLine($"Consumed {track.TrackId} with processed result = {processResult} ? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
        // Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");
        
        // LinkedList<Processor<>> people = new LinkedList<string>(new[] { "Tom", "Sam", "Bob" });
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

    private void PrintThreadInfo()
    {
        // Get the number of threads currently in use by the thread pool.
        ThreadPool.GetAvailableThreads(out int workerThreads, out int cpThreads);

// Print the number of threads to the console.
        Console.WriteLine("There are {0} threads currently in use by the thread pool.", workerThreads);
    }
}