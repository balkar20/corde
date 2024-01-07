using TrafficControlApp.Config;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class DangerAnalyzerService(VehicleDangerAnalyseConfig vehicleDangerAnalyseConfig) : IAnalyzerService
{
    private  TimeSpan timeForAnalyse = vehicleDangerAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new DangerAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing item Danger: Id={analysingItem.ItemId} with Danger=..."
        };
    }
}