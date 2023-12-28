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
{

    #region private fields

    protected readonly ISharedMemoryVehicleService _sharedMemoryService;
    protected readonly IVehicleAnalyzerService<IAnalysingResult> _vehicleAnalyzerService;
    protected readonly IMapper _mapper;

    #endregion
    
    #region Public Properties

        public bool CanRun { get; set; }
        
        public string ProcessorId { get; set; }
        
        public string ProcessId { get; set; }
        
        public string InputId { get; set; }

        public bool CompletedWithDependentProcessors { get; set; }
        
        public Queue<IProcessor<TInput>> ProcessorsDepended { get; set; }
        
    
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
            await CurrentCallingProcessorInCaseQueueIsEmpty.ProcessNextAsync(inputData);
            return;
        }

        await ProcessLogic(inputData);
        var nextInQueProcessor = GetNextProcessor();

        if (nextInQueProcessor == null)
        {
            HandleProcessorCompletion();
        }

        CurrentCallingProcessorInCaseQueueIsEmpty = nextInQueProcessor;
    }
    
    
    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        if (ProcessorsDepended == null)
        {
            ProcessorsDepended = new Queue<IProcessor<TInput>>();
        }
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    private IProcessor<TInput> GetNextProcessor()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        return proc;
    }

    private void HandleProcessorCompletion()
    {
        CompletedWithDependentProcessors = true;
        SetUpNextTrackProcessor();
    }

    private void SetUpNextTrackProcessor()
    {
        this.CompletedWithDependentProcessors = false;
        this.CurrentCallingProcessorInCaseQueueIsEmpty = this;
    }

    #endregion
}