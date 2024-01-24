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

public class VehicleBorrowProcessor
    : ProgressiveProcessor<Track, VehicleBorrowProcessionResult>
{
    private readonly IProcessingItemsStorageServiceRepository<string, Track, VehicleBorrowProcessionResult> processingItemsStorageServiceRepository;
    private readonly IAnalyzerService analyzerService;
    private readonly IMapper mapper;
    private readonly IEventLoggingService loggingService;
    private readonly string processorName;
    public VehicleBorrowProcessor(IProcessingItemsStorageServiceRepository<string, Track, VehicleBorrowProcessionResult> processingItemsStorageServiceRepository,
        IAnalyzerService analyzerService,
        IMapper mapper,
        IEventLoggingService loggingService, 
        string processorName)
        : base(loggingService, processorName)
    {
        this.processingItemsStorageServiceRepository = processingItemsStorageServiceRepository;
        this.analyzerService = analyzerService;
        this.mapper = mapper;
        this.loggingService = loggingService;
        this.processorName = processorName;
    }
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