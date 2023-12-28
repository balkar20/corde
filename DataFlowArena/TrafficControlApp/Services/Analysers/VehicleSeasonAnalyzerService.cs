using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services.Analysers;

class VehicleSeasonAnalyzerService : IVehicleAnalyzerService<IAnalysingResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleSeasonAnalyzerService(VehicleSeasonAnalyseConfig vehicleSeasonAnalyseConfig)
    {
        this.timeForAnalyse = vehicleSeasonAnalyseConfig.TimeForAnalyse;
    }

    public async Task<IAnalysingResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new SeasonAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Season: Number={vehicle.VehicleNumber} with Season=...."
        };
    }
}