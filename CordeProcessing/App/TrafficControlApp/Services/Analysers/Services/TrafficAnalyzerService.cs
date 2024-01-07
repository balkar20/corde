using TrafficControlApp.Config;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace TrafficControlApp.Services.Analysers.Services;

class TrafficAnalyzerService(VehicleTrafficAnalyseConfig vehicleTrafficAnalyseConfig) : IAnalyzerService
{
    private  TimeSpan timeForAnalyse = vehicleTrafficAnalyseConfig.TimeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new TrafficAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing vehicle Traffic: Number={analysingItem.ItemId} with Traffic=...."
        };
    }
}