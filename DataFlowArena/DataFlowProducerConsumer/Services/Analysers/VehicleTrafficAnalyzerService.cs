using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Models.Results.Analyse;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services.Analysers;

class VehicleTrafficAnalyzerService : IVehicleAnalyzerService<TrafficAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleTrafficAnalyzerService(VehicleTrafficAnalyseConfig vehicleTrafficAnalyseConfig)
    {
        this.timeForAnalyse = vehicleTrafficAnalyseConfig.TimeForAnalyse;
    }

    public async Task<TrafficAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new TrafficAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Traffic: Number={vehicle.VehicleNumber} with Traffic=...."
        };
    }
}