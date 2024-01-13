using System.Collections.Concurrent;

namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInputData>
{
    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }
    public bool IsStartedSelfProcessing { get; set; }
    string InputId { get; set; }
    bool IsRoot { get; set; }
    bool IsEventCompletionFired {get; set; }
    
    ConcurrentStack<IProcessor<TInputData>> ProcessorsExecuting { get; set; }
    string ProcessorTypeName { get; set; }
    IProcessor<TInputData>? ParentProcessor { get; set; }
    IProcessor<TInputData>? RootProcessorFromDependentQueue { get; set; }
    ConcurrentQueue<IProcessor<TInputData>> DependedProcessors { get; set; }
    int DependentProcessorsExecutingCount { get; set; }
    // IProcessor<TInputData>? NextInQueueProcessor { get; set; }
    Task ProcessNextAsync(TInputData inputData);
    Task DoConditionalProcession(TInputData inputData);
    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
    event NotifyNestedProcessingCompleted NestedProcessingCompletedEvent;
    event NotifyCurrentProcessingCompleted CurrentProcessingCompletedEvent;
    event NotifyParentProcessingCompleted ParentProcessingCompletedEvent;
    delegate Task NotifyNestedProcessingCompleted();
    delegate Task NotifyCurrentProcessingCompleted(IProcessor<TInputData> processor);
    delegate Task NotifyParentProcessingCompleted(TInputData inputData);

    Task  FireCurrentProcessingCompletedEvent(IProcessor<TInputData> inputData);
    // IProcessor<TInput> GetNextProcessor()
}