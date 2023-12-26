using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models;

namespace DataFlowProducerConsumer.Producers.Abstraction;

public interface IProducer
{
    Task ProduceAllAsync(ITargetBlock<Track> target);
}