using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class VehicleColorAnalyzerService : IVehicleAnalyzerService<IAnalysingResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleColorAnalyzerService(VehicleColorAnalyseConfig vehicleColorAnalyseConfig)
    {
        this.timeForAnalyse = vehicleColorAnalyseConfig.TimeForAnalyse;
    }

    public async Task<IAnalysingResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new ColorAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle color: Number={vehicle.VehicleNumber} with Color={vehicle.VehicleColor}"
        };
    }
}