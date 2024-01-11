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

public class VehicleTypeProcessor
    (IProcessingItemsStorageServiceRepository<string, Track, VehicleTypeProcessionResult> processingItemsStorageServiceRepository,
    IAnalyzerService analyzerService,
    IMapper mapper,
    IEventLoggingService eventLoggingService)
    : Processor<Track, VehicleTypeProcessionResult>(eventLoggingService)
{
    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        //But what if roots a lot?
        var resultOfRoot = await processingItemsStorageServiceRepository.GetProcessingItemResult(inputData.ItemId);
        
        //Here we need some aggregated procession result
        // VehicleTypeProcessionResult r = resultOfRoot;
        
        //Work with resultOf Root further......
     
        var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        var typeProcessionResult = mapper.Map<VehicleTypeProcessionResult>(typeAnaliseResult);
        await eventLoggingService.LogEvent($"TYPE + time {DateTime.Now}", EventLoggingTypes.ProcessedEvent);

        return typeProcessionResult;
    }

    protected override async Task SetProcessionResult(VehicleTypeProcessionResult result)
    {
        await processingItemsStorageServiceRepository.CreateProcessingItemResult(result);
    }
}