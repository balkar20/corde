using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Consumers.Abstractions;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Consumers;

class TrackConsumer : ITrackConsumer
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
            track => _processorPool.ProcessAsync(track),
            new ExecutionDataflowBlockOptions 
            { BoundedCapacity = config.BoundedCapacity,
                MaxDegreeOfParallelism = config.MaxParallelConsumeCount });
        buffer.LinkTo(consumerBlock, new DataflowLinkOptions() 
            { PropagateCompletion = config.PropagateCompletion });

        startProducing(consumerBlock);

        await consumerBlock.Completion;
    }
}