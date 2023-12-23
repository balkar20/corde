using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Processors;

public class VehicleTypeProcessor: IProcessor<Track, VehicleTypeProcessResult>
{
    public async Task<VehicleTypeProcessResult> ProcessAsync(ISourceBlock<VehicleTypeProcessResult> source)
    {
        int bytesProcessed = 0;
        VehicleTypeProcessResult result = new VehicleTypeProcessResult()
        {
            IsSuccessed = false,
        };
        
        while (await source.OutputAvailableAsync())
        {
            result = await source.ReceiveAsync();
        }

        return result;
    }
}