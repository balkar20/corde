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

public class VehicleTrafficProcessor(IProcessingItemsStorageServiceRepository<string, Track, VehicleTrafficProcessionResult> processingItemsStorageServiceRepository,
    IAnalyzerService analyzerService,
    IMapper mapper,
    IEventLoggingService eventLoggingService)
    : Processor<Track, VehicleTrafficProcessionResult>(eventLoggingService)
{
    
    // public VehicleTrafficProcessor(ISharedMemoryVehicleService sharedMemoryService,
    //     IVehicleAnalyzerService<IAnalysingResult> vehicleAnalyzerService, 
    //     IMapper mapper) : 
    //     base(sharedMemoryService, vehicleAnalyzerService, mapper)
    // {
    // }

    #region Protected Methods

     protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
        {
            var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
            var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
            var typeProcessionResult = mapper.Map<VehicleTrafficProcessionResult>(typeAnaliseResult);
            await eventLoggingService.LogEvent("TRAFFIC PROCESSED!");
            return typeProcessionResult;
        }

        protected override async Task SetProcessionResult(VehicleTrafficProcessionResult result)
        {
            await processingItemsStorageServiceRepository.CreateProcessingItemResult(result);
        }

        #endregion

   
    #region Private Methods

    private async Task WorkWithDependentData(string trackId)
    {
        // sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessionResult dependentData);
        // Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }

    #endregion
}