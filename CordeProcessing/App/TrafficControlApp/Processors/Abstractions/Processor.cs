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

    public int TotalAmountOfProcessors { get; set; } = 1;

    public string ProcessorTypeName { get; set; }
    public string ProcessorName { get; set; } = processorName;

    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }

    public bool IsStartedSelfProcessing { get; set; }
    
    public bool GotDependentProcessorsExecutingCountFromDependentRoot { get; set; }
    
    public IProcessor<TInput>? RootProcessorFromDependentQueue { get; set; }
    public IProcessor<TInput>? ParentProcessor { get; set; }

    #endregion

    #region Events

    public event Func<Task> NestedProcessingCompletedEvent;
    public event Func<IProcessor<TInput>, int, Task> CurrentProcessingCompletedEvent;
    
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
        var isCurrentTreadForCompletedRoot = IsStartedSelfProcessing && IsCompletedCurrentProcessing && IsRoot;
        var isCurrentTreadForNotStartedExecutionRoot =
            !IsStartedSelfProcessing && !IsCompletedCurrentProcessing && IsRoot;
        //This block executing only from other from root threads and
        //!!!there is No sense to set here something for Semaphore!!!
        if (!IsRoot)
        {
            IsStartedSelfProcessing = true;
            await ProcessLogicAndComplete(inputData);
            IsCompletedCurrentProcessing = true;
            return;
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
        if (ParentProcessor != null)
        {
            ParentProcessor.TotalAmountOfProcessors++;
        }

        TotalAmountOfProcessors++;
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
            parentProcessor.TotalAmountOfProcessors -= count;
            return parentProcessor.DecrementParentsTotalCount(count, parentProcessor.ParentProcessor);
        }
        
        TotalAmountOfProcessors -= count;
        if (TotalAmountOfProcessors == -1)
        {
            this.NestedProcessingCompletedEvent?.Invoke();
        }

        return TotalAmountOfProcessors;
    }
    
    public void SetDependents(ConcurrentQueue<IProcessor<TInput>> dependents)
    {
        DependedProcessors = dependents;
        TotalAmountOfProcessors += dependents.Count;
        IncrementParentsTotalCount(dependents.Count, ParentProcessor);
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
        await ProcessLogicAndComplete(inputData);

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

            //Here if we can see that RootProcessorFromDependentQueue
            //exhausted With the his dependencies And achieve the last one which is root - Then we swap it to that  
            // if (RootProcessorFromDependentQueue.DependedProcessors.TryPeek(out var dependedProcessor) &&
            //     dependedProcessor.IsRoot)
            // {
            //     RootProcessorFromDependentQueue = dependedProcessor;
            // }
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

        return proc;
    }

    private async Task ProcessLogicAndComplete(TInput input)
    {
        await ProcessLogic(input);
        // TotalAmountOfProcessors--;
        DecrementParentsTotalCount(1, this.ParentProcessor);
        // if (DecrementParents(1, this) == 0)
        // {
        //     this.NestedProcessingCompletedEvent?.Invoke();
        // }
        //
        // if (CurrentProcessingCompletedEvent != null)
        // {
        //     CurrentProcessingCompletedEvent(this, ProcessorId);
        // }
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