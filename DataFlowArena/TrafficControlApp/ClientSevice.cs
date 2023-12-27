using System.Threading.Tasks.Dataflow;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Processors;

namespace TrafficControlApp;

public class ClientSevice
{
    public async Task Execute()
    {
        // var buffer = new BufferBlock<VehicleTypeProcessResult>();
        // var typeProcessor = new VehicleTypeProcessor();
        // await typeProcessor.ProcessAsync(buffer);
    }
}