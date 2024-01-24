using ParallelProcessing.Configuration;
using ParallelProcessing.Models.Items.Analysing;
using ParallelProcessing.Models.Results.Analyse;
using ParallelProcessing.Models.Results.Analyse.Abstractions;
using ParallelProcessing.Services.Analysers.Abstractions;

namespace ParallelProcessing.Services.Analysers.Services;

class DangerAnalyzerService : IAnalyzerService
{
    public DangerAnalyzerService(VehicleDangerAnalyseConfig vehicleTypeAnalyseConfig)
    {
        this.timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;
    }
    private TimeSpan timeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new DangerAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing item Danger: Id={analysingItem.ItemId} with Danger=..."
        };
    }
}