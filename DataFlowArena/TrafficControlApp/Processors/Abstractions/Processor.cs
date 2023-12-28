using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Storage;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput>(ISharedMemoryVehicleService _sharedMemoryService,
        IVehicleAnalyzerService<IAnalysingResult> _vehicleAnalyzerService, IMapper _mapper)
    : IProcessor<TInput> 
    // where TOutput : IProcessResult, new()
{
   protected readonly ISharedMemoryVehicleService _sharedMemoryService;
   protected readonly IVehicleAnalyzerService<IAnalysingResult> _vehicleAnalyzerService;
   protected readonly IMapper _mapper;
    // public IVehicleAnalyzerService<IAnalysingResult> _vehicleAnalyzerService { get; } = vehicleAnalyzerService;

    #region private fields

    // protected readonly ISharedMemoryVehicleService _sharedMemoryService = sharedMemoryService;
    // protected readonly IVehicleAnalyzerService<IAnalysingResult> _vehicleAnalyzerService = vehicleAnalyzerService;
    // protected readonly IMapper _mapper = mapper;

    #endregion
    #region Constructors

    #endregion
    
    #region Public Properties

        public bool CanRun { get; set; }
        
        public string ProcessId { get; set; }
        
        public string InputId { get; set; }

        public bool CompletedWithDependentProcessors { get; set; }
        
        public Queue<IProcessor<TInput>?> ProcessorsDepended { get; set; }
        
    
        public IProcessor<TInput> CurrentCallingProcessorInCaseQueueIsEmpty { get; set; }
    
        public bool NextInQueHasNoDependencies { get; set; }

    #endregion
    
    
    
    #region Protected Abstract Methods

    protected abstract Task<IProcessResult> ProcessLogic(TInput inputData);

    #endregion
    
    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        if (CompletedWithDependentProcessors)
        {
            // await CurrentCallingProcessorInCaseQueueIsEmpty.ProcessLogic(inputData);
            // await CurrentCallingProcessorInCaseQueueIsEmpty.ProcessNext(inputData);
            await ProcessLogic(inputData);
            return;
        }

        await ProcessLogic(inputData);
        var nextInQueProcessor = GetNextProcessor();

        // NextInQueHasNoDependencies  = !nextInQueProcessor.ProcessorsDepended.TryPeek(out var afterHim);
        if (nextInQueProcessor == null)
        {
        }

        CurrentCallingProcessorInCaseQueueIsEmpty = nextInQueProcessor;

        CompletedWithDependentProcessors = true;
    }
    
    
    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        // 1. Some validation for processor
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    private IProcessor<TInput> GetNextProcessor()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        return proc;
    }

    #endregion
}