using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Services.Analysers;

class VehicleMarkAnalyzerService(VehicleMarkAnalyseConfig vehicleMarkAnalyseConfig)
    : IVehicleAnalyzerService<VehicleMarkStatisticsProcessResult>
{
    private  TimeSpan timeForAnalyse = vehicleMarkAnalyseConfig.TimeForAnalyse;

    public async Task<VehicleMarkStatisticsProcessResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new VehicleMarkStatisticsProcessResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle mark: Number={vehicle.VehicleNumber} with Mark={vehicle.VehicleMark}"
        };
    }
}