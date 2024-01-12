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
    string ProcessorName { get; set; }
    IProcessor<TInputData>? ParentProcessor { get; set; }
    IProcessor<TInputData>? RootProcessorFromDependentQueue { get; set; }
    // IProcessor<TInputData>? NextInQueueProcessor { get; set; }
    Task ProcessNextAsync(TInputData inputData);
    Task DoConditionalProcession(TInputData inputData, bool isReleased);
    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
    event NotifyNestedProcessingCompleted NestedProcessingCompletedEvent;
    event NotifyCurrentProcessingCompleted CurrentProcessingCompletedEvent;
    event NotifyParentProcessingCompleted ParentProcessingCompletedEvent;
    delegate Task NotifyNestedProcessingCompleted();
    delegate void NotifyCurrentProcessingCompleted(TInputData inputData);
    delegate Task NotifyParentProcessingCompleted(TInputData inputData);

    void FireCurrentProcessingCompletedEvent(TInputData inputData);
    // IProcessor<TInput> GetNextProcessor()
}