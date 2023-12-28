using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services.Analysers;

class VehicleDangerAnalyzerService : IVehicleAnalyzerService<IAnalysingResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleDangerAnalyzerService(VehicleDangerAnalyseConfig vehicleDangerAnalyseConfig)
    {
        this.timeForAnalyse = vehicleDangerAnalyseConfig.TimeForAnalyse;
    }

    public async Task<IAnalysingResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new DangerAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Danger: Number={vehicle.VehicleNumber} with Danger=..."
        };
    }
}