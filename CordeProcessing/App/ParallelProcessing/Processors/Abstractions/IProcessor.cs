using System.Collections.Concurrent;

namespace ParallelProcessing.Processors.Abstractions;

public interface IProcessor<TInput>
{
    //Properties
    bool IsCompletedNestedProcessing { get;  }
    bool IsCompletedSelfProcessing { get; set; }
    bool IsStartedSelfProcessing { get; set; }
    bool IsRoot { get; set; }
    bool IsDependantRoot { get; set; }
    int TotalAmountOfProcessors {get; set; }
    
    bool IsSomeOfNestedRootsProcessingCompletedEventFired {get; set; }
    
    
    
    ConcurrentStack<IProcessor<TInput>> ProcessorsExecuting { get; set; }
    string ProcessorTypeName { get; set; }
    string ProcessorName { get; set; }
    int DependentProcessorsExecutingCount { get; set; }
    IProcessor<TInput>? ParentProcessor { get; set; }
    IProcessor<TInput>? RootProcessorFromDependentQueue { get; set; }
    ConcurrentQueue<IProcessor<TInput>> DependedProcessors { get; set; }
    ConcurrentQueue<IProcessor<TInput>>? RootsFromDependantQueuePool { get; }
    bool GotDependentProcessorsExecutingCountFromDependentRoot { get; set; }

    Task ProcessNextAsync(TInput inputData);
    Task DoConditionalProcession(TInput inputData);
    void AddDependentProcessor(IProcessor<TInput> dependentProcessor);
    void SetDependents(ConcurrentQueue<IProcessor<TInput>> dependents);
    int IncrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor);
    int DecrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor);
    void RecursivelySetRootProcessorForDependentQueueToParent(IProcessor<TInput> processor, IProcessor<TInput> parentProcessor);
    void RecursivelySetRootProcessorForDependentQueuePoolToParent(IProcessor<TInput> processor, IProcessor<TInput> parentProcessor);
    Task SignalNestedProcessingCompletionEvent();
    Task SignalDependantProcessorWasDequeuedEvent();
    Task SignalSomeOfNestedRootsProcessingCompletedEvent();
    
    //Events
    event Func<Task> NestedProcessingCompletedEvent;
    event Func<Task> SomeOfNestedRootsProcessingCompletedEvent;
    event Func<Task> DependantProcessorWasDequeuedEvent;
    event Func<Task> CurrentProcessingCompletedEvent;
    event Func<TInput, Task> ParentProcessingCompletedEvent;
}