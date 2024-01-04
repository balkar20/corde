using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class ColorAnalyzerService(VehicleColorAnalyseConfig vehicleColorAnalyseConfig) : IAnalyzerService
{
    private  TimeSpan timeForAnalyse = vehicleColorAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new ColorAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing Item color: ItemId={analysingItem.ItemId} with Color="
        };
    }
}