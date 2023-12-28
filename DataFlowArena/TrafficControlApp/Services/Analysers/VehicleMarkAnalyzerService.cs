using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;

namespace TrafficControlApp.Services.Analysers;

class VehicleMarkAnalyzerService(VehicleMarkAnalyseConfig vehicleMarkAnalyseConfig)
    : IVehicleAnalyzerService<IAnalysingResult>
{
    private  TimeSpan timeForAnalyse = vehicleMarkAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new MarkAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle mark: Number={vehicle.VehicleNumber} with Mark={vehicle.VehicleMark}"
        };
    }
}