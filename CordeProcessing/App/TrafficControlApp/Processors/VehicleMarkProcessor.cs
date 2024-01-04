using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Processors;

public class VehicleMarkProcessor
    (IProcessingItemsStorageServiceRepository<string, Track> processingItemsStorageServiceRepository,
        IAnalyzerService analyzerService,
        IMapper mapper,
        IEventLoggingService eventLoggingService)
    : Processor<Track>(eventLoggingService)
{
    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        var resultOfRoot = await processingItemsStorageServiceRepository.GetProcessingItemResult(inputData.ItemId);

        var isSucceed = resultOfRoot.IsSucceed;
        
        var analysingItem = mapper.Map<AnalysingItem>(inputData);
        await WorkWithDependentData(inputData.ItemId);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        var result =  mapper.Map<VehicleMarkProcessionResult>(typeAnaliseResult);
        // sharedMemoryService.VehicleMarkProcessResultDictionary.Add(inputData.ItemId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
         // // sharedMemoryService.VehicleColorStatisticsProcessResultDictionary.TryGetValue(trackId, out VehicleColorProcessionResult dependentData);
         // Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}