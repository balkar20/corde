using AutoMapper;
using ParallelProcessing.Models;
using ParallelProcessing.Models.Items.Analysing;
using ParallelProcessing.Models.Results;
using ParallelProcessing.Models.Results.Procession.Abstractions;
using ParallelProcessing.Processors.Abstractions;
using ParallelProcessing.Services;
using ParallelProcessing.Services.Analysers.Abstractions;
using ParallelProcessing.Services.Events.Abstractions;
using ParallelProcessing.Services.Events.Data.Enums;

namespace ParallelProcessing.Processors;

public class VehicleTrafficProcessor : ProgressiveProcessor<Track, VehicleTrafficProcessionResult>
{
    private readonly IProcessingItemsStorageServiceRepository<string, Track, VehicleTrafficProcessionResult> processingItemsStorageServiceRepository;
    private readonly IAnalyzerService analyzerService;
    private readonly IMapper mapper;
    private readonly IEventLoggingService loggingService;
    private readonly string processorName;

    public VehicleTrafficProcessor(IProcessingItemsStorageServiceRepository<string, Track, VehicleTrafficProcessionResult> processingItemsStorageServiceRepository,
        IAnalyzerService analyzerService,
        IMapper mapper,
        IEventLoggingService loggingService, 
        string processorName): base (loggingService, processorName)
    {
        this.processingItemsStorageServiceRepository = processingItemsStorageServiceRepository;
        this.analyzerService = analyzerService;
        this.mapper = mapper;
        this.loggingService = loggingService;
        this.processorName = processorName;
    }


    #region Protected Methods

    protected override async Task<IProcessionResult> ProcessLogic(Track inputData)
        {
            var analysingItem = mapper.Map<TypeAnalysingItem>(inputData);
            var typeAnaliseResult = await analyzerService.Analyse(analysingItem);
            var typeProcessionResult = mapper.Map<VehicleTrafficProcessionResult>(typeAnaliseResult);
            await loggingService.Log($"{ProcessorName} + time {DateTime.Now}", EventLoggingTypes.ProcessedProcessor);
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