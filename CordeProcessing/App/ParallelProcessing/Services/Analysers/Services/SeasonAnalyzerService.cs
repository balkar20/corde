using ParallelProcessing.Configuration;
using ParallelProcessing.Models.Items.Analysing;
using ParallelProcessing.Models.Results.Analyse;
using ParallelProcessing.Models.Results.Analyse.Abstractions;
using ParallelProcessing.Services.Analysers.Abstractions;

namespace ParallelProcessing.Services.Analysers.Services;

class SeasonAnalyzerService : IAnalyzerService
{
    public SeasonAnalyzerService(VehicleSeasonAnalyseConfig vehicleTypeAnalyseConfig)
    {
        this.timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;
    }
    private readonly TimeSpan timeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(timeForAnalyse);
        return new SeasonAnalyseResult
        {
            Message = $"I was Delayed for {timeForAnalyse} by Analysing Item Season: Number={analysingItem.ItemId} with Season=...."
        };
    }
}