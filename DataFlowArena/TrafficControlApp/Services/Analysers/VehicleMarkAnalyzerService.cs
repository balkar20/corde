using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;

namespace TrafficControlApp.Services.Analysers;

class VehicleMarkAnalyzerService(VehicleMarkAnalyseConfig vehicleMarkAnalyseConfig)
    : IVehicleAnalyzerService<MarkAnalyseResult>
{
    private  TimeSpan timeForAnalyse = vehicleMarkAnalyseConfig.TimeForAnalyse;

    public async Task<MarkAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new MarkAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle mark: Number={vehicle.VehicleNumber} with Mark={vehicle.VehicleMark}"
        };
    }
}