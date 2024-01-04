using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class SeasonAnalyzerService(VehicleSeasonAnalyseConfig vehicleSeasonAnalyseConfig) : IAnalyzerService
{
    private  TimeSpan timeForAnalyse = vehicleSeasonAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new SeasonAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing Item Season: Number={analysingItem.ItemId} with Season=...."
        };
    }
}