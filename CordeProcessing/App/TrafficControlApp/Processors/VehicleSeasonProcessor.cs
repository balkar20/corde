using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Processors;

public class VehicleSeasonProcessor(IProcessingItemsStorageServiceRepository<string, Track, VehicleSeasonProcessionResult> processingItemsStorageServiceRepository,
    IAnalyzerService analyzerService,
    IMapper mapper,
    IEventLoggingService eventLoggingService)
    : Processor<Track, VehicleSeasonProcessionResult>(eventLoggingService)
{

    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        // var vehicles = await _sharedMemoryService.GetVehicleDataByTrackId(inputData.ItemId);
        //
        // foreach (var vehicle in vehicles)
        // {
        //     var vehicleType = vehicle.VehicleType;
        //     
        // }
        
        var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        var typeProcessionResult = mapper.Map<VehicleSeasonProcessionResult>(typeAnaliseResult);
        return typeProcessionResult;
    }
    
    
    protected override async Task SetProcessionResult(VehicleSeasonProcessionResult result)
    {
        await processingItemsStorageServiceRepository.CreateProcessingItemResult(result);
    }

    private async Task WorkWithDependentData(string trackId)
    {
         // _sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessionResult dependentData);
         // Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}