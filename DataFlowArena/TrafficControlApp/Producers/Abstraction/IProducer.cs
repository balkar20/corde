using System.Threading.Tasks.Dataflow;
using TrafficControlApp.Models;

namespace TrafficControlApp.Producers.Abstraction;

public interface IProducer
{
    Task ProduceAllAsync(ITargetBlock<Track> target);
}