using System.Collections.Concurrent;
using System.Collections.Frozen;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TrafficControlApp.Contexts;
using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Analyse;
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

    private ConcurrentQueue<IProcessor<TInput>> ProcessorsDepended = new ();
    
    private SyncContext<TInput> semaphoreSlim = new ();
    private int counter = 0;
    private bool locked;

    #endregion

    #region Public Properties

    public ConcurrentStack<IProcessor<TInput>> ProcessorsExecuting { get; set; } = new ();
    public string ProcessorId  => Guid.NewGuid().ToString();
    
    public string InputId { get; set; }
    public int TreadId { get; set; }
    public bool IsRoot { get; set; }
    
    public bool IsEventCompletionFired { get; set; }
    public string ProcessorName { get; set; }
    
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
        ProcessorName = this.GetType().FullName;
        CurrentProcessingCompletedEvent += HandleCompletionEvent;
        var threadId = Thread.CurrentThread.ManagedThreadId;
        TreadId = threadId;
        await LoggingService.Log(threadId.ToString(), EventLoggingTypes.ThreadIdLogging, $"|||{ProcessorName}||| with dependant: {RootProcessorFromDependentQueue?.ProcessorName}");
        await LoggingService.Log($"Time {DateTime.Now}", EventLoggingTypes.ProcessionInformation, $"|||{ProcessorName}");

        var isReleased = await semaphoreSlim.WaitLock(this);
        TreadId = threadId;
        // if (TreadId != 0 && TreadId != threadId)
        // {
        //     CurrentProcessingCompletedEvent += async (o) => await HandleCompletionEvent(inputData);
        //     TreadId = threadId;
        // }
        // if (!IsEventCompletionFired)
        // {
            await DoConditionalProcession(inputData, isReleased);
        // }
    }

    public void FireCurrentProcessingCompletedEvent(TInput inputData)
    {
        CurrentProcessingCompletedEvent(inputData);
    }


    public async Task DoConditionalProcession(TInput inputData, bool isReleased)
    {
        try
        {
            var existsInQueueForNextProcessing = false;
            //This block executing only from other from root threads and
            //!!!there is No sense to set here something for Semaphore!!!
            if (!IsRoot)
            {
                IsStartedSelfProcessing = true;
                await ProcessLogic(inputData);
                IsCompletedCurrentProcessing = true;
                // IsEventCompletionFired = false;
                return;
            }
            
            //This block Executing in Root thread and only in case Root is fully completed
            if (IsStartedSelfProcessing && IsCompletedCurrentProcessing && IsRoot)
            {
                var nextInQueProcessor = GetNextProcessorFromDependants();
                // IF we have root processor set from que????????? Then we execute it 
                if (nextInQueProcessor == null && RootProcessorFromDependentQueue != this && RootProcessorFromDependentQueue != null)
                {
                    await RootProcessorFromDependentQueue.DoConditionalProcession(inputData, isReleased);
                    IsCompletedCurrentProcessing = true;
                    return;
                }
                //iN case we handle completedProcessionEvent for non root processors there will be null here and we just need to return  
                if (nextInQueProcessor == null)
                {
                    await LoggingService.Log("nextInQueProcessor = null", EventLoggingTypes.ExceptionKindEvent, ProcessorName);
                    return;
                }
                //DEBUG - that is a strange case
                if (!IsRoot)
                {
                    await LoggingService.Log("!!!!!!!!!!!!!!!!!!!!!!No Root!!!!!!!!!!!!!!!!!!!!!!!!!!", EventLoggingTypes.ExceptionKindEvent, ProcessorName);
                }
                //If we dont have root processor set from queue and remain in queue processor => Then we execute queue processor
                if(nextInQueProcessor != null)
                {
                    await nextInQueProcessor.DoConditionalProcession(inputData, isReleased);
                    
                    //Here we check Dependents and push for avoid LOCK
                    ProcessorsDepended.TryPeek(out IProcessor<TInput>? proc);
                    if(proc != null) ProcessorsExecuting.Push(proc);
                    
                    IsCompletedCurrentProcessing = true;
                    // await nextInQueProcessor.DoConditionalProcession(inputData);
                }
            }
            
            //This block Executing in Root thread and only in case Root is Never Called
            //So it executing once for root??????
            if (!IsStartedSelfProcessing && !IsCompletedCurrentProcessing && IsRoot)
            {
                IsStartedSelfProcessing = true;
                //So here first that was in Queue processor we got in previous thread need to be ProcessNextAsync 
                if (RootProcessorFromDependentQueue != null && RootProcessorFromDependentQueue != this)
                {
                    // await RootProcessorFromDependentQue.DoConditionalProcession(inputData, true);
                    await RootProcessorFromDependentQueue.ProcessNextAsync(inputData);
                }
                else
                {
                    await ProcessLogicForRootsBeforeStartCallingDepependencies(inputData, existsInQueueForNextProcessing);
                }

                if (RootProcessorFromDependentQueue == null)   
                {
                    
                }

                if (RootProcessorFromDependentQueue == this)   
                {
                    
                }
                IsCompletedCurrentProcessing = true;
            }
        }
        finally
        {
            if (!isReleased)
            {
                // IsEventCompletionFired = false;
                semaphoreSlim.ReleaseLock(this);
            }
            // IsEventCompletionFired = false;
        }
    }

    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        ProcessorsDepended.Enqueue(dependentProcessor);
        this.IsRoot = true;
        // dependentProcessor.ParentProcessingCompletedEvent += ParentProcessorOnCurrentProcessingCompletedEventHandler;
        dependentProcessor.ParentProcessor = this;
    }

    #endregion


    #region Protected Methods

    #endregion


    #region Private Methods

    private async Task ProcessLogicForRootsBeforeStartCallingDepependencies(TInput inputData, bool existsInQueueForNextProcessing)
    {
        // if (existsInQueueForNextProcessing)
        // {
        //Here we check Dependents and push for avoid LOCK
        if (ProcessorsDepended.TryPeek(out var executingNextProcessor))
            ProcessorsExecuting.Push(executingNextProcessor);

        // }
        //Here we just ProcessLogic because it root for someone 
        await ProcessLogic(inputData);
                    
        if (ParentProcessor != null)
        {
            //Here we fire event for locked threads pass through Semaphore and continue execute
            //in parallel but for parent in case current processor is dependent and root simultaneously
            // ParentProcessor.FireCurrentProcessingCompletedEvent(inputData);
            ParentProcessor.RootProcessorFromDependentQueue = this; 
            return;
        }
        //Here we fire event for locked threads pass through Semaphore and continue execute in parallel
        // FireCurrentProcessingCompletedEvent(inputData);
    }
    
    
    private IProcessor<TInput>? GetNextProcessorFromDependants()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        if (proc == null)
        {
            IsCompletedNestedProcessing = true;
        }

        return proc;
    }

    private bool CheckIsThereProcessorNextInQueueAndGetHim()
    {
        ;
        if (ProcessorsDepended.TryPeek(out IProcessor<TInput>? proc))
        {
            return true;
        }

        return false;
    }

    // public void SetDependents(Queue<IProcessor<TInput>> dependents)
    // {
    //     ProcessorsDepended = dependents;
    // }

    #endregion
    
    
    #region Event Handlers
    
    private async Task ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler(IProcessor<TInput> processor)
    {
        await LoggingService.Log($"ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler on {this.ProcessorName}", EventLoggingTypes.HandlingEvent, processor.ProcessorName);
    }

    private async Task NestedProcessingCompletedEventHandler()
    {
        await LoggingService.Log("NestedProcessingCompletedEventHandler", EventLoggingTypes.HandlingEvent);
    }

    private void HandleCompletionEvent(TInput inputData)
    {
        var currentTreadId = Thread.CurrentThread.ManagedThreadId;
        
        if (TreadId != 0 && currentTreadId != TreadId)
        {
            // IsEventCompletionFired = true;
            // IsCompletedCurrentProcessing = true;
            Task.Run(async () => await DoConditionalProcession(inputData, false) );
            // Task.Run(DoConditionalProcession(inputData, false));
        }
        
    }

    #endregion
}