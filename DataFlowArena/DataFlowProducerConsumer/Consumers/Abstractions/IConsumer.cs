using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models;

namespace DataFlowProducerConsumer.Consumers.Abstractions;

public interface IConsumer<TBufferData>
{
    // ITargetBlock<TBufferData> ConsumeAllAsync(BufferBlock<TBufferData> buffer);
    Task ConsumeAllAsync(ISourceBlock<Track> buffer,
        Func<ITargetBlock<Track>, Task> startProducing);
}