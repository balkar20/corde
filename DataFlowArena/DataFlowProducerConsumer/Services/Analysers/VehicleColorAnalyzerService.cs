using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Models.Results.Analyse;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services.Analysers;

class VehicleColorAnalyzerService : IVehicleAnalyzerService<ColorAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleColorAnalyzerService(VehicleColorAnalyseConfig vehicleColorAnalyseConfig)
    {
        this.timeForAnalyse = vehicleColorAnalyseConfig.TimeForAnalyse;
    }

    public async Task<ColorAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new ColorAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle color: Number={vehicle.VehicleNumber} with Color={vehicle.VehicleColor}"
        };
    }
}