using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services;

class VehicleMarkStatisticsAnalyzerService(VehicleTypeAnalyseConfig vehicleTypeAnalyseConfig)
    : IVehicleAnalyzerService<VehicleMarkStatisticsProcessResult>
{
    private  TimeSpan timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;

    public async Task<VehicleMarkStatisticsProcessResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new VehicleMarkStatisticsProcessResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle mark: Number={vehicle.VehicleNumber} with type={vehicle.VehicleType}"
        };
    }
}