using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Models.Results.Analyse;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services.Analysers;

class VehicleDangerAnalyzerService : IVehicleAnalyzerService<DangerAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleDangerAnalyzerService(VehicleDangerAnalyseConfig vehicleDangerAnalyseConfig)
    {
        this.timeForAnalyse = vehicleDangerAnalyseConfig.TimeForAnalyse;
    }

    public async Task<DangerAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new DangerAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Danger: Number={vehicle.VehicleNumber} with Danger=..."
        };
    }
}