using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

public class TypeAnalyzerService(VehicleTypeAnalyseConfig vehicleTypeAnalyseConfig) : IAnalyzerService
{
    private  TimeSpan _timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(_timeForAnalyse);
        return new TypeAnalyseResult
        {
            Message = $"I was Delayed for {_timeForAnalyse} by Analysing vehicle type: Number={analysingItem.ItemId} with type="
        };
    }
}