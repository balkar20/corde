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

public class VehicleBorrowProcessor(IProcessingItemsStorageServiceRepository<string, Track, VehicleBorrowProcessionResult> processingItemsStorageServiceRepository,
    IAnalyzerService analyzerService,
    IMapper mapper,
    IEventLoggingService loggingService, 
    string processorName)
    : Processor<Track, VehicleBorrowProcessionResult>(loggingService, processorName)
{

    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        var typeProcessionResult = mapper.Map<VehicleBorrowProcessionResult>(typeAnaliseResult);

        await loggingService.Log($"{ProcessorName} + time {DateTime.Now}", EventLoggingTypes.ProcessedProcessor);
        return typeProcessionResult;
    }
    
    
    protected override async Task SetProcessionResult(VehicleBorrowProcessionResult result)
    {
        await processingItemsStorageServiceRepository.CreateProcessingItemResult(result);
    }

    private async Task WorkWithDependentData(string trackId)
    {
         // _sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessionResult dependentData);
         // Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}