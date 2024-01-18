using System.Collections.Concurrent;
using TrafficControlApp.Contexts;
using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput, TProcessionResult>(
    IEventLoggingService? loggingService,
    string processorName)
    : IProcessor<TInput>
    where TInput : ApplicationItem<string>
    where TProcessionResult : IProcessionResult
{
    #region private fields

    private readonly IEventLoggingService? LoggingService = loggingService;

    private readonly ParallelProcessionSynchronizationService<TInput> _parallelProcessionSynchronizationService = new(loggingService);

    #endregion

    #region Public Properties

    public ConcurrentStack<IProcessor<TInput>> ProcessorsExecuting { get; set; } = new();

    public int DependentProcessorsExecutingCount { get; set; }

    public ConcurrentQueue<IProcessor<TInput>> DependedProcessors { get; set; } = new();
    
    public bool IsRoot { get; set; }
    
    public bool IsDependantRoot { get; set; }

    public int TotalAmountOfProcessors { get; set; } = 1;

    public string ProcessorTypeName { get; set; }
    public string ProcessorName { get; set; } = processorName;

    public bool IsCompletedNestedProcessing { get;  }
    public bool IsCompletedSelfProcessing { get; set; }

    public bool IsStartedSelfProcessing { get; set;}
    public bool IsSomeOfNestedRootsProcessingCompletedEventFired { get; set; }
    
    public bool GotDependentProcessorsExecutingCountFromDependentRoot { get; set; }
    
    public IProcessor<TInput>? RootProcessorFromDependentQueue { get; set; }
    public IProcessor<TInput>? ParentProcessor { get; set; }
    
    public ConcurrentQueue<IProcessor<TInput>>? RootsFromDependantQueuePool { get; } = new ();

    #endregion

    #region Events

    public event Func<Task> NestedProcessingCompletedEvent;
    public event Func<Task> DependantProcessorWasDequeuedEvent;
    public event Func<Task> SomeOfNestedRootsProcessingCompletedEvent;
    public event Func<Task> CurrentProcessingCompletedEvent;
    
    #endregion

    #region Protected Abstract Methods

    protected abstract Task<IProcessionResult> ProcessLogic(TInput inputData);
    
    protected abstract Task SetProcessionResult(TProcessionResult result);

    #endregion

    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        await _parallelProcessionSynchronizationService.WaitLockWithCallback(this, DoConditionalProcession, inputData);
    }

    public async Task DoConditionalProcession(TInput inputData)
    {
        var isCurrentTreadForCompletedRoot = IsStartedSelfProcessing && IsCompletedSelfProcessing && IsRoot;
        var isCurrentTreadForNotStartedExecutionRoot =
            !IsStartedSelfProcessing && !IsCompletedSelfProcessing && IsRoot;
        //This block executing only from other from root threads and
        //!!!there is No sense to set here something for Semaphore!!!
        if (!IsRoot)
        {
            IsStartedSelfProcessing = true;
            SignalDependantProcessorWasDequeuedEvent();
            await ProcessLogicAndComplete(inputData);
            IsCompletedSelfProcessing = true;
            return;
        }
        
                
        //This block Executing in Root thread and only in case Root is Never Called
        //So it executing once for root
        if (isCurrentTreadForNotStartedExecutionRoot)
        {
            await ProcessLogicForRootBeforeStartCallingDependencies(inputData);
            return;
        }

        
        //This block Executing in Root thread and only in case Root is fully completed
        if (isCurrentTreadForCompletedRoot)
        {
            await CheckAndProcessDependentProcessorOrDependantRoot(inputData);
        }
    }

    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        ProcessorTypeName = this.GetType().FullName;
        dependentProcessor.ProcessorTypeName = dependentProcessor.GetType().FullName;
        // if (DependedProcessors.TryPeek(out var lastDependantInParent))
        // {
        //     lastDependantInParent.IsHaveToPassNextRoot = true;
        // }

        DependedProcessors.Enqueue(dependentProcessor);

        TotalAmountOfProcessors++;
        DependentProcessorsExecutingCount++;
        IncrementParentsTotalCount(1, ParentProcessor);
        this.IsRoot = true;

        dependentProcessor.ParentProcessor = this;
    }

    public event Func<TInput, Task>? ParentProcessingCompletedEvent;

    public int IncrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor)
    {
        if (parentProcessor != null)
        {
            parentProcessor.TotalAmountOfProcessors += count;
            return parentProcessor.IncrementParentsTotalCount(count, parentProcessor.ParentProcessor);
        }

        return TotalAmountOfProcessors;
    }

    public int DecrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor)
    {

        if (parentProcessor != null)
        {
            parentProcessor.DecrementParentsTotalCount(count, parentProcessor.ParentProcessor);
            parentProcessor.TotalAmountOfProcessors -= count;
        }
        
        // TotalAmountOfProcessors -= count;
        

        return TotalAmountOfProcessors;
    }
    
    public void SetDependents(ConcurrentQueue<IProcessor<TInput>> dependents)
    {
        this.IsRoot = true;
        DependedProcessors = dependents;
        TotalAmountOfProcessors += dependents.Count;
        IncrementParentsTotalCount(dependents.Count, ParentProcessor);
    }

    public async Task SignalNestedProcessingCompletionEvent()
    {
        await NestedProcessingCompletedEvent?.Invoke();
    }

    public async Task SignalDependantProcessorWasDequeuedEvent()
    {
        await DependantProcessorWasDequeuedEvent?.Invoke();
    }

    public async Task SignalSomeOfNestedRootsProcessingCompletedEvent()
    {
        if (!IsSomeOfNestedRootsProcessingCompletedEventFired)
        {
            await SomeOfNestedRootsProcessingCompletedEvent?.Invoke();
            IsSomeOfNestedRootsProcessingCompletedEventFired = true;
        }
    }
    
    public void RecursivelySetRootProcessorForDependentQueueToParent(IProcessor<TInput> processor, IProcessor<TInput> parentProcessor)
    {
        if (processor.IsRoot && parentProcessor == null)
        {
            return;
        }
        if (parentProcessor?.ParentProcessor == null)
        {
            parentProcessor.RootProcessorFromDependentQueue = processor;
            parentProcessor.GotDependentProcessorsExecutingCountFromDependentRoot = false;
            return;
        }
        if (parentProcessor != null)
        {
            RecursivelySetRootProcessorForDependentQueueToParent(processor, parentProcessor.ParentProcessor);
            return;
        }
    }
    
    public void RecursivelySetRootProcessorForDependentQueuePoolToParent(IProcessor<TInput> processor, IProcessor<TInput> parentProcessor)
    {
        if (processor.IsRoot && parentProcessor == null)
        {
            return;
        }
        if (parentProcessor?.ParentProcessor == null)
        {
            parentProcessor.RootsFromDependantQueuePool.Enqueue(processor);
            processor.IsDependantRoot = true;
            
            
            parentProcessor.GotDependentProcessorsExecutingCountFromDependentRoot = false;
            return;
        }
        if (parentProcessor != null)
        {
            RecursivelySetRootProcessorForDependentQueuePoolToParent(processor, parentProcessor.ParentProcessor);
            return;
        }
    }
    
    public void RecursivelySubscribeRootProcessorCompletedEventParent(IProcessor<TInput> processor, IProcessor<TInput> parentProcessor)
    {
        if (processor.IsRoot && parentProcessor == null)
        {
            return;
        }
        if (parentProcessor?.ParentProcessor == null)
        {
            parentProcessor.IsSomeOfNestedRootsProcessingCompletedEventFired = false;
            processor.CurrentProcessingCompletedEvent += async () => await parentProcessor
                .SignalSomeOfNestedRootsProcessingCompletedEvent();
            // parentProcessor.SomeOfNestedRootsProcessingCompletedEvent+= async () => await SignalSomeOfNestedRootsProcessingCompletedEvent();
            
            parentProcessor.GotDependentProcessorsExecutingCountFromDependentRoot = false;
            return;
        }
        if (parentProcessor != null)
        {
            RecursivelySetRootProcessorForDependentQueuePoolToParent(processor, parentProcessor.ParentProcessor);
            return;
        }
    }

    #endregion
    
    #region Private Methods

    private async Task ProcessLogicForRootBeforeStartCallingDependencies(TInput inputData)
    {
        //Here we check Dependents and set ProcessorsExecutingCount for avoid LOCK
        // if (DependedProcessors.Any())
        // {
        //     DependentProcessorsExecutingCount = DependedProcessors.Count;
        // }

        //Here we just ProcessLogic because it root for some Dependencies
        
   
        // this.CurrentProcessingCompletedEvent +=  async () => await SignalSomeOfNestedRootsProcessingCompletedEvent();
        RecursivelySubscribeRootProcessorCompletedEventParent(this, this.ParentProcessor);
        await ProcessLogicAndComplete(inputData);
        RecursivelySetRootProcessorForDependentQueuePoolToParent(this, this.ParentProcessor);
    }

    private async Task CheckAndProcessDependentProcessorOrDependantRoot(TInput inputData)
    {
        var nextInQueueProcessor = GetNextProcessorFromDependants();

        // IF   root processor than was  set from queue during parallel execution, then  execute it 
        // if (RootsFromDependantQueuePool.TryTake(out var rootProcessorFromPool))
        // {
        //     await rootProcessorFromPool.DoConditionalProcession(inputData);
        //     if (rootProcessorFromPool.DependedProcessors.TryPeek(out var pp))
        //     {
        //         RootsFromDependantQueuePool.Add(rootProcessorFromPool);
        //     }
        //     
        //     return;
        // }
        if (RootProcessorFromDependentQueue != null && RootProcessorFromDependentQueue != this)
        {
            await RootProcessorFromDependentQueue.DoConditionalProcession(inputData);
            if (RootProcessorFromDependentQueue.DependedProcessors.Count > 0)
            {
                RecursivelySetRootProcessorForDependentQueuePoolToParent(RootProcessorFromDependentQueue, ParentProcessor);
            }
            return;
        }
        
        // if (RootProcessorFromDependentQueue != null && 
        //          nextInQueProcessor == null)
        // {
        //     if (nextInQueProcessor != null)
        //     {
        //         RootProcessorFromDependentQueue.IsHaveToPassNextRoot = true;
        //         RootsFromDependantQueuePool.Add(nextInQueProcessor);
        //     }
        //     await RootProcessorFromDependentQueue.DoConditionalProcession(inputData);
        //     // RootProcessorFromDependentQueue.IsHaveToPassNextRoot = false;
        //     // if (RootProcessorFromDependentQueue.DependedProcessors.TryPeek(out var dp) && dp.IsRoot)
        //     // {
        //     //     RootsFromDependantQueuePool.Add(dp);
        //     // }
        //     return;
        // }

        // if (RootProcessorFromDependentQueue != this && RootProcessorFromDependentQueue != null)
        // {
        //     await RootProcessorFromDependentQueue.DoConditionalProcession(inputData);
        //     return;
        // }

        //If we dont have root processor set from queue and remain in queue processors => Then we execute queue processor
        if (nextInQueueProcessor != null)
        {
            // if (DependedProcessors.TryPeek(out var nextInQueueProcessorAfter) && nextInQueueProcessorAfter.IsRoot)
            // {
            //     nextInQueueProcessor.IsHaveToPassNextRoot = true;
            // }
            
            await nextInQueueProcessor.DoConditionalProcession(inputData);
        }
    }


    private IProcessor<TInput>? GetNextProcessorFromDependants()
    {
        DependedProcessors.TryDequeue(out IProcessor<TInput>? proc);
        
        return proc;
    }

    private async Task ProcessLogicAndComplete(TInput input)
    {
        IsStartedSelfProcessing = true;
        await ProcessLogic(input);
        
        IsCompletedSelfProcessing = true;
        CurrentProcessingCompletedEvent?.Invoke();
        var totalCount = DecrementParentsTotalCount(1, this.ParentProcessor);
        if (totalCount == 0)
        {
            
        }
        
        TotalAmountOfProcessors--;
    }

    #endregion


    #region Event Handlers

    private async Task ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler(IProcessor<TInput> processor)
    {
        await LoggingService.Log(
            $"ProcessorFromDependentQueOnCurrentProcessingCompletedEventHandler on {this.ProcessorTypeName}",
            EventLoggingTypes.HandlingEvent, processor.ProcessorTypeName);
    }

    private async Task NestedProcessingCompletedEventHandler()
    {
        // await LoggingService.Log("NestedProcessingCompletedEventHandler", EventLoggingTypes.);
    }

    #endregion
}