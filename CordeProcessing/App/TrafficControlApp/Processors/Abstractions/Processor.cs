using System.Collections.Concurrent;
using TrafficControlApp.Contexts;
using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput, TProcessionResult>(
        IEventLoggingService? loggingService)
    : IProcessor<TInput>
where TInput: ApplicationItem<string>
where TProcessionResult: IProcessionResult
{
    #region private fields
    
    private readonly IEventLoggingService? LoggingService = loggingService;

    private readonly SyncContext<TInput> _syncContext = new (loggingService);

    #endregion

    #region Public Properties

    public ConcurrentStack<IProcessor<TInput>> ProcessorsExecuting { get; set; } = new ();
    
    public int DependentProcessorsExecutingCount { get; set; }
    
    public ConcurrentQueue<IProcessor<TInput>> DependedProcessors { get; set; } = new ();
    
    public string ProcessorId  => Guid.NewGuid().ToString();
    
    
    public string InputId { get; set; }
    public int TreadId { get; set; }
    public bool IsRoot { get; set; }
    
    public bool IsEventCompletionFired { get; set; }
    public string ProcessorTypeName { get; set; }
    
    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }
    
    public bool IsStartedSelfProcessing { get; set; }

    public IProcessor<TInput>? RootProcessorFromDependentQueue { get; set; }
    public IProcessor<TInput>? ParentProcessor { get; set; }

    #endregion
    
    #region Events

    public event IProcessor<TInput>.NotifyNestedProcessingCompleted NestedProcessingCompletedEvent;
    public event IProcessor<TInput>.NotifyCurrentProcessingCompleted CurrentProcessingCompletedEvent;
    public event IProcessor<TInput>.NotifyParentProcessingCompleted ParentProcessingCompletedEvent;
        
    #endregion

    #region Protected Abstract Methods

    protected abstract Task<IProcessionResult> ProcessLogic(TInput inputData);
    
    
    protected abstract Task SetProcessionResult(TProcessionResult result);

    #endregion

    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        TreadId = threadId;
        await LoggingService.Log(threadId.ToString(), EventLoggingTypes.ThreadIdLogging, $"|||{ProcessorTypeName}||| with dependant: {RootProcessorFromDependentQueue?.ProcessorTypeName}");
        await LoggingService.Log($"Time {DateTime.Now}", EventLoggingTypes.ProcessionInformation, $"|||{ProcessorTypeName}");
        
        await _syncContext.WaitLockWithCallback(this, DoConditionalProcession, inputData);
    }

    public async Task DoConditionalProcession(TInput inputData)
    {
        var isCurrentTreadForCompletedRoot = IsStartedSelfProcessing && IsCompletedCurrentProcessing && IsRoot;
        var isCurrentTreadForNotStartedExecutionRoot =
            !IsStartedSelfProcessing && !IsCompletedCurrentProcessing && IsRoot;
        //This block executing only from other from root threads and
        //!!!there is No sense to set here something for Semaphore!!!
        if (!IsRoot)
        {
            IsStartedSelfProcessing = true;
            await ProcessLogic(inputData);
            IsCompletedCurrentProcessing = true;
            return;
        }
        else
        {
        }
        
        //This block Executing in Root thread and only in case Root is Never Called
        //So it executing once for root
        if (isCurrentTreadForNotStartedExecutionRoot)
        {
            IsStartedSelfProcessing = true;
            await ProcessLogicForRootBeforeStartCallingDependencies(inputData);
            IsCompletedCurrentProcessing = true;
            return;
        }

        //This block Executing in Root thread and only in case Root is fully completed
        if (isCurrentTreadForCompletedRoot)
        {
            await CheckAndProcessDependentProcessor(inputData);
        }
    }

    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        ProcessorTypeName = this.GetType().FullName;
        dependentProcessor.ProcessorTypeName = dependentProcessor.GetType().FullName;
        DependedProcessors.Enqueue(dependentProcessor);
        this.IsRoot = true;
        // dependentProcessor.ParentProcessingCompletedEvent += ParentProcessorOnCurrentProcessingCompletedEventHandler;
        dependentProcessor.ParentProcessor = this;
    }
    
    public async Task FireCurrentProcessingCompletedEvent(IProcessor<TInput> inputData)
    {
        await CurrentProcessingCompletedEvent(inputData,1);
    }

    #endregion


    #region Protected Methods

    #endregion


    #region Private Methods

    private async Task ProcessLogicForRootBeforeStartCallingDependencies(TInput inputData)
    {
        //Here we check Dependents and set ProcessorsExecutingCount for avoid LOCK
        if (DependedProcessors.Any())
        {
                DependentProcessorsExecutingCount = DependedProcessors.Count;
        }
         
        //Here we just ProcessLogic because it root for some Dependencies
        await ProcessLogic(inputData);
        
        if (ParentProcessor != null)
        {
            ParentProcessor.RootProcessorFromDependentQueue = this; 
        }
    }

    private async Task CheckAndProcessDependentProcessor(TInput inputData)
    {
        var nextInQueProcessor = GetNextProcessorFromDependants();
        // IF   root processor than was  set from queue during parallel execution, then  execute it 
        if (RootProcessorFromDependentQueue != this && RootProcessorFromDependentQueue != null)
        {
            await RootProcessorFromDependentQueue.DoConditionalProcession(inputData);
            return;
        }

        //If we dont have root processor set from queue and remain in queue processors => Then we execute queue processor
        if (nextInQueProcessor != null)
        {
            await nextInQueProcessor.DoConditionalProcession(inputData);
        }
    }
    
    
    private IProcessor<TInput>? GetNextProcessorFromDependants()
    {
        DependedProcessors.TryDequeue(out IProcessor<TInput>? proc);
        if (proc == null)
        {
            IsCompletedNestedProcessing = true;
        }

        return proc;
    }

    public void SetDependents(ConcurrentQueue<IProcessor<TInput>> dependents)
    {
        DependedProcessors = dependents;
    }

    #endregion
    
    
    #region Event Handlers
    
    private async Task ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler(IProcessor<TInput> processor)
    {
        await LoggingService.Log($"ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler on {this.ProcessorTypeName}", EventLoggingTypes.HandlingEvent, processor.ProcessorTypeName);
    }

    private async Task NestedProcessingCompletedEventHandler()
    {
        await LoggingService.Log("NestedProcessingCompletedEventHandler", EventLoggingTypes.HandlingEvent);
    }

    #endregion
}