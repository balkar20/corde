using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services.Analysers;

public class VehicleTypeAnalyzerService : IVehicleAnalyzerService<IAnalysingResult>
{
    private  TimeSpan _timeForAnalyse;

    public VehicleTypeAnalyzerService(VehicleTypeAnalyseConfig vehicleTypeAnalyseConfig)
    {
        this._timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;
    }

    public async Task<IAnalysingResult> Analyse(Vehicle vehicle)
    {
        await Task.Delay(_timeForAnalyse);
        return new TypeAnalyseResult
        {
            Message = $"I was Delayed for {_timeForAnalyse} by Analysing vehicle type: Number={vehicle.VehicleNumber} with type={vehicle.VehicleType}"
        };
    }
}