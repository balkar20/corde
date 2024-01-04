using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Processors;

public class VehicleColorProcessor(IProcessingItemsStorageServiceRepository<string, Track> processingItemsStorageServiceRepository,
        IAnalyzerService analyzerService,
        IMapper mapper,
        IEventLoggingService eventLoggingService)
    : Processor<Track>(eventLoggingService)
{

    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
    {
        
        var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
        var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
        var typeProcessionResult = mapper.Map<VehicleTypeProcessionResult>(typeAnaliseResult);
        return typeProcessionResult;
    }

    private async Task WorkWithDependentData(string trackId)
    {
        
    }
}