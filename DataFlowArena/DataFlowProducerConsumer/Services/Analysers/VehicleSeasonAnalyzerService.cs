using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Models.Results.Analyse;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services.Analysers;

class VehicleSeasonAnalyzerService : IVehicleAnalyzerService<SeasonAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleSeasonAnalyzerService(VehicleSeasonAnalyseConfig vehicleSeasonAnalyseConfig)
    {
        this.timeForAnalyse = vehicleSeasonAnalyseConfig.TimeForAnalyse;
    }

    public async Task<SeasonAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new SeasonAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Season: Number={vehicle.VehicleNumber} with Season=...."
        };
    }
}