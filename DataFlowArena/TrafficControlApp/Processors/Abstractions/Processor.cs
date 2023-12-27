using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Storage;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput, TAnalyserResult> : IProcessor<TInput> 
    // where TOutput : IProcessResult, new()
{
    #region private fields

    protected readonly ISharedMemoryVehicleService _sharedMemoryService;
    protected readonly IVehicleAnalyzerService<TAnalyserResult> _vehicleAnalyzerService;
    protected readonly IMapper _mapper;

    #endregion
    #region Constructors

    public Processor(ISharedMemoryVehicleService sharedMemoryService, IVehicleAnalyzerService<TAnalyserResult> vehicleAnalyzerService, IMapper mapper)
    {
        _sharedMemoryService = sharedMemoryService;
        _vehicleAnalyzerService = vehicleAnalyzerService;
        _mapper = mapper;
    }

    #endregion
    
    #region Public Properties

        public bool CanRun { get; set; }
        
        public string ProcessId { get; set; }
        
        public string InputId { get; set; }

        public bool CompletedWithDependentProcessors { get; set; }
        
        public Queue<Processor<TInput, IAnalysingResult>?> ProcessorsDepended { get; set; }
        
    
        public Processor<TInput, IAnalysingResult> CurrentCallingProcessorInCaseQueueIsEmpty { get; set; }
    
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
            await CurrentCallingProcessorInCaseQueueIsEmpty.ProcessLogic(inputData);
            return;
        }

        await ProcessLogic(inputData);
        var nextInQueProcessor = GetNextProcessor();

        NextInQueHasNoDependencies  = !nextInQueProcessor.ProcessorsDepended.TryPeek(out var afterHim);
        if (nextInQueProcessor == null)
        {
        }

        CurrentCallingProcessorInCaseQueueIsEmpty = nextInQueProcessor;

        CompletedWithDependentProcessors = true;
    }
    
    
    public void AddDependentProcessor(Processor<TInput, IAnalysingResult> dependentProcessor)
    {
        // 1. Some validation for processor
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    private Processor<TInput, IAnalysingResult> GetNextProcessor()
    {
        ProcessorsDepended.TryDequeue(out Processor<TInput, IAnalysingResult>? proc);
        return proc;
    }

    #endregion
}