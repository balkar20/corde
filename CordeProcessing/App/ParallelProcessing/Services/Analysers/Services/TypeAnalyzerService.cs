using ParallelProcessing.Configuration;
using ParallelProcessing.Models.Items.Analysing;
using ParallelProcessing.Models.Results.Analyse;
using ParallelProcessing.Models.Results.Analyse.Abstractions;
using ParallelProcessing.Services.Analysers.Abstractions;

namespace ParallelProcessing.Services.Analysers.Services;

public class TypeAnalyzerService : IAnalyzerService
{
    public TypeAnalyzerService(VehicleTypeAnalyseConfig vehicleTypeAnalyseConfig)
    {
        this._timeForAnalyse = vehicleTypeAnalyseConfig.TimeForAnalyse;
    }
    private  readonly TimeSpan _timeForAnalyse;

    public async Task<IAnalysingResult> Analyse(AnalysingItem analysingItem)
    {
        await Task.Delay(_timeForAnalyse);
        return new TypeAnalyseResult
        {
            Message = $"I was Delayed for {_timeForAnalyse} by Analysing vehicle type: Number={analysingItem.ItemId} with type="
        };
    }
}