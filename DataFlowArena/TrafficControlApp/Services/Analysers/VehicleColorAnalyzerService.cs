using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services.Analysers;

class VehicleColorAnalyzerService : IVehicleAnalyzerService<ColorAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleColorAnalyzerService(VehicleColorAnalyseConfig vehicleColorAnalyseConfig)
    {
        this.timeForAnalyse = vehicleColorAnalyseConfig.TimeForAnalyse;
    }

    public async Task<ColorAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new ColorAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle color: Number={vehicle.VehicleNumber} with Color={vehicle.VehicleColor}"
        };
    }
}