using System.Threading.Tasks.Dataflow;
using TrafficControlApp.ClientDevices.Abstractions;
using TrafficControlApp.Config;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Models;
using TrafficControlApp.Producers.Abstraction;

namespace TrafficControlApp.Root;

public class TrafficFlowProcessStarter(
    ITrackDevice trackDevice,
    ITrackProducer trackProducer,
    ITrackConsumer trackConsumer, 
    ApplicationConfiguration configuration)
{
    protected TimeSpan consumeSpeed => configuration.ConsumeSpeed;
    private  int maxParallelConsume => configuration.MaxParallelConsumeCount;

    // [Benchmark]
    public async Task StartProcess()
    {
        var buffer = new BufferBlock<Track>(
            new DataflowBlockOptions() { BoundedCapacity = maxParallelConsume });
        
        await trackConsumer.ConsumeAllAsync(buffer, block => 
            trackProducer.ProduceAllAsync(block));
    }
}