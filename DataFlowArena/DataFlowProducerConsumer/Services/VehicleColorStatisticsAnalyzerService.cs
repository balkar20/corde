using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services;

class VehicleColorStatisticsAnalyzerService : IVehicleAnalyzerService<TypeAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleColorStatisticsAnalyzerService(VehicleTypeAnalyseConfig vehicleTypeAnalyseConfig)
    {
        this.timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;
    }

    public async Task<TypeAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new TypeAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle color: Number={vehicle.VehicleNumber} with type={vehicle.VehicleType}"
        };
    }
}