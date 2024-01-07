using System.Collections.Frozen;
using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput>(
        IEventLoggingService? eventLoggingService)
    : IProcessor<TInput>
where TInput: ApplicationItem<string>
{
    #region private fields

    // protected readonly IAnalyzerService _analyzerService = _analyzerService;
    // protected readonly IProcessingItemsStorageServiceRepository<string> _processingItemsStorageServiceRepository = _processingItemsStorageServiceRepository;
    // protected readonly IMapper _mapper = _mapper;
    protected readonly IEventLoggingService? EventLoggingService = eventLoggingService;
    
    protected  Queue<IProcessor<TInput>> ProcessorsDepended = new ();
    protected readonly Queue<IProcessor<TInput>> ProcessorsDependedCopy = new ();

    #endregion

    #region Public Properties

    public bool CanRun { get; set; }

    public string ProcessorId  => Guid.NewGuid().ToString();
    
    public string InputId { get; set; }

    // public bool CompletedWithDependentProcessors { get; set; }
    
    public bool CompletedProcessing { get; set; }

    public bool HasDependants => (bool)ProcessorsDepended?.Any();

    public IProcessor<TInput> CurrentCallingProcessorInCaseQueueIsEmpty { get; set; }
    public IProcessor<TInput> ProcessorFromDependentQue { get; set; }

    public bool NextInQueHasNoDependencies { get; set; }

    #endregion


    #region Protected Abstract Methods

    protected abstract Task<IProcessionResult> ProcessLogic(TInput inputData);

    #endregion

    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        try
        {
            if (this.InputId != inputData.ItemId)
            {
                EventLoggingService?.LogEvent($"ProcessNextAsync New item Id: {inputData.ItemId}");
                this.InputId = inputData.ItemId;
            } 
            if (ProcessorFromDependentQue != null)
            {
                await ProcessorFromDependentQue.ProcessNextAsync(inputData);
                
                SetNextProcessorFromDependent(ProcessorFromDependentQue);
                return;
            }
            
            
            var processionResult = await ProcessLogic(inputData);
            SetNextProcessor();
            await EventLoggingService.LogEvent($"PS: {this.ProcessorId} | ItemId: {this.InputId}, {this.ProcessorId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void SetNextProcessorFromDependent(IProcessor<TInput> processorFromDependentQue)
    {
        if (processorFromDependentQue.CompletedProcessing)
        {
            ProcessorFromDependentQue = GetNextProcessorFromDependants();
        }
    }

    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        // if (ProcessorsDepended == null)
        // {
        //     ProcessorsDepended = new Queue<IProcessor<TInput>>();
        // }
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    
    private void ReconStructForNextBatch()
    {
        throw new NotImplementedException();
    }

    protected IProcessor<TInput> GetNextProcessorFromDependants()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        if (proc == null)
        {
            CompletedProcessing = true;
        }

        return proc;
    }
    
    protected void SetNextProcessor()
    {
        ProcessorFromDependentQue = GetNextProcessorFromDependants();
    }

    void GetSubProcessor()
    {
        return;
    }

    public void SetDependents(Queue<IProcessor<TInput>> dependents)
    {
        ProcessorsDepended = dependents;
    }

    public Queue<IProcessor<TInput>>  GetDependents()
    {
        return ProcessorsDepended;
    }

    // private async Task HandleProcessorCompletionWithDependencies()
    // {
    //     await EventLoggingService.LogEvent(
    //         $"PS: {this.ProcessorId}  HandleProcessorCompletion| ItemId: {this.InputId}, {this.ProcessorId}");
    //     // CompletedWithDependentProcessors = true;
    //     await SetUpNextItemProcessor();
    // }
    //
    // private async Task SetUpNextItemProcessor()
    // {
    //     await EventLoggingService.LogEvent("Setting Up ");
    //     // this.CompletedWithDependentProcessors = false;
    //     this.CurrentCallingProcessorInCaseQueueIsEmpty = this;
    // }

    #endregion
}