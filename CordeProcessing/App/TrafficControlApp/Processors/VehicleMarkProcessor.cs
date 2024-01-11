using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Processors;

public class VehicleMarkProcessor
    (IProcessingItemsStorageServiceRepository<string, Track, VehicleMarkProcessionResult> processingItemsStorageServiceRepository,
        IAnalyzerService analyzerService,
        IMapper mapper,
        IEventLoggingService eventLoggingService)
    : Processor<Track, VehicleMarkProcessionResult>(eventLoggingService)
{
    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        // var resultOfRoot = await processingItemsStorageServiceRepository.GetProcessingItemResult(inputData.ItemId);
        //
        // var isSucceed = resultOfRoot.IsSucceed;
        
        var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
        await WorkWithDependentData(inputData.ItemId);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        // var typeProcessionResult = 
        var result =  mapper.Map<VehicleMarkProcessionResult>(typeAnaliseResult);
        // sharedMemoryService.VehicleMarkProcessResultDictionary.Add(inputData.ItemId, result);
        await eventLoggingService.LogEvent($"MARK + time {DateTime.Now}", EventLoggingTypes.ProcessedEvent);

        return result;
    }

    protected override async Task SetProcessionResult(VehicleMarkProcessionResult result)
    {
        await processingItemsStorageServiceRepository.CreateProcessingItemResult(result);
    }

    private async Task WorkWithDependentData(string trackId)
    {
         // // sharedMemoryService.VehicleColorStatisticsProcessResultDictionary.TryGetValue(trackId, out VehicleColorProcessionResult dependentData);
         // Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}