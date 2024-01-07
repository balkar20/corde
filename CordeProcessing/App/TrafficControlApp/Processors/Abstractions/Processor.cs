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
    
    protected readonly Queue<IProcessor<TInput>> ProcessorsDepended = new ();
    protected readonly Queue<IProcessor<TInput>> ProcessorsDependedCopy = new ();

    #endregion

    #region Public Properties

    public bool CanRun { get; set; }

    public string ProcessorId  => Guid.NewGuid().ToString();
    
    public string InputId { get; set; }

    public bool CompletedWithDependentProcessors { get; set; }
    
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
            // ProcessorsDepended.CopyTo(ProcessorsDependedCopy);
            if (this.InputId != inputData.ItemId)
            {
                EventLoggingService?.LogEvent($"ProcessNextAsync New item Id: {inputData.ItemId}");
                this.InputId = inputData.ItemId;
            } 
            if (ProcessorFromDependentQue != null)
            {
                await ProcessorFromDependentQue.ProcessNextAsync(inputData);
                ProcessorFromDependentQue.CompletedProcessing = true;
                SetNextProcessor();
                // if (ProcessorFromDependentQue.CompletedProcessing && !ProcessorFromDependentQue.HasDependants)
                // {
                //     ProcessorFromDependentQue = GetNextProcessor();
                // }
                // else
                // {
                //     ProcessorFromDependentQue = ((Processor<TInput>)ProcessorFromDependentQue).GetNextProcessor();
                // }
                
                return;
            }
            
            
            var processionResult = await ProcessLogic(inputData);
            CompletedProcessing = true;
            SetNextProcessor();
            await EventLoggingService.LogEvent($"PS: {this.ProcessorId} | ItemId: {this.InputId}, {this.ProcessorId}");

            // var nextInQueProcessor = GetNextProcessor();
            // ProcessorFromDependentQue = nextInQueProcessor;
            // if (nextInQueProcessor == null)
            // {
            //     await HandleProcessorCompletionWithDependencies();
            //     return;
            // }
            
            // CurrentCallingProcessorInCaseQueueIsEmpty = nextInQueProcessor;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
    
    protected IProcessor<TInput> GetNextProcessor()
    {
        // IProcessor<TInput> proc = null;
        // if (ProcessorFromDependentQue.CompletedProcessing && !ProcessorFromDependentQue.HasDependants)
        // {
        //     ProcessorFromDependentQue = GetNextProcessor();
        // }
        // else
        // {
            ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        
        return proc;
    }
    
    protected void SetNextProcessor()
    {
        // IProcessor<TInput> proc = null;void
        if (ProcessorFromDependentQue == null || (ProcessorFromDependentQue.CompletedProcessing && !ProcessorFromDependentQue.HasDependants))
        {
            ProcessorFromDependentQue = GetNextProcessor();
        }
        else
        {
            ProcessorFromDependentQue = ((Processor<TInput>)ProcessorFromDependentQue).ProcessorFromDependentQue;
        }
    }

    private async Task HandleProcessorCompletionWithDependencies()
    {
        await EventLoggingService.LogEvent(
            $"PS: {this.ProcessorId}  HandleProcessorCompletion| ItemId: {this.InputId}, {this.ProcessorId}");
        // CompletedWithDependentProcessors = true;
        await SetUpNextItemProcessor();
    }

    private async Task SetUpNextItemProcessor()
    {
        await EventLoggingService.LogEvent("Setting Up ");
        this.CompletedWithDependentProcessors = false;
        this.CurrentCallingProcessorInCaseQueueIsEmpty = this;
    }

    #endregion
}