using System.Threading.Tasks.Dataflow;
using TrafficControlApp.Config;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Models;
using TrafficControlApp.Processors;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Consumers;

public class TrackConsumer : ITrackConsumer
{
    public SortedSet<int> Treads { get; set; }
    private IProcessor<Track> _processorPool;
    private ApplicationConfiguration config;

    public TrackConsumer(ApplicationConfiguration config, IProcessor<Track> processorPool)
    {
        this.config = config;
        _processorPool = processorPool;
    }
    
    public async Task ConsumeAllAsync(ISourceBlock<Track> buffer, Func<ITargetBlock<Track>, Task> startProducing)
    {
        var consumerBlock = new ActionBlock<Track>(
            track => _processorPool.ProcessNextAsync(track),
            new ExecutionDataflowBlockOptions 
            { BoundedCapacity = config.BoundedCapacity,
                MaxDegreeOfParallelism = config.MaxParallelConsumeCount });
        buffer.LinkTo(consumerBlock, new DataflowLinkOptions() 
            { PropagateCompletion = config.PropagateCompletion });

        startProducing(consumerBlock);

        await consumerBlock.Completion;
    }
}