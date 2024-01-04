using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class MarkAnalyzerService(VehicleMarkAnalyseConfig vehicleMarkAnalyseConfig)
    : IAnalyzerService
{
    private TimeSpan timeForAnalyse => vehicleMarkAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new MarkAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing Item mark: ItemId={analysingItem.ItemId} with Mark="
        };
    }
}