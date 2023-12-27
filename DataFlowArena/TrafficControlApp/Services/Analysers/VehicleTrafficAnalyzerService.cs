using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services.Analysers;

class VehicleTrafficAnalyzerService : IVehicleAnalyzerService<TrafficAnalyseResult>
{
    private  TimeSpan timeForAnalyse;

    public VehicleTrafficAnalyzerService(VehicleTrafficAnalyseConfig vehicleTrafficAnalyseConfig)
    {
        this.timeForAnalyse = vehicleTrafficAnalyseConfig.TimeForAnalyse;
    }

    public async Task<TrafficAnalyseResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(timeForAnalyse);
        return new TrafficAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Traffic: Number={vehicle.VehicleNumber} with Traffic=...."
        };
    }
}