using System.Collections.Concurrent;

namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInput>
{
    //Properties
    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }
    public bool IsStartedSelfProcessing { get; set; }
    bool IsRoot { get; set; }
    int TotalAmountOfProcessors {get; set; }
    
    
    ConcurrentStack<IProcessor<TInput>> ProcessorsExecuting { get; set; }
    string ProcessorTypeName { get; set; }
    string ProcessorName { get; set; }
    int DependentProcessorsExecutingCount { get; set; }
    IProcessor<TInput>? ParentProcessor { get; set; }
    IProcessor<TInput>? RootProcessorFromDependentQueue { get; set; }
    ConcurrentQueue<IProcessor<TInput>> DependedProcessors { get; set; }
    
    Task ProcessNextAsync(TInput inputData);
    Task DoConditionalProcession(TInput inputData);
    void AddDependentProcessor(IProcessor<TInput> dependentProcessor);
    void SetDependents(ConcurrentQueue<IProcessor<TInput>> dependents);
    int IncrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor);
    int DecrementParentsTotalCount(int count, IProcessor<TInput> parentProcessor);
    
    //Events
    event Func<Task> NestedProcessingCompletedEvent;
    event Func<IProcessor<TInput>, int, Task> CurrentProcessingCompletedEvent;
    event Func<TInput, Task> ParentProcessingCompletedEvent;
}