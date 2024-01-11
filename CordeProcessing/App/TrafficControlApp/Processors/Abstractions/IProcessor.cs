namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInputData>
{
    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }
    public bool IsStartedSelfProcessing { get; set; }
    string InputId { get; set; }
    bool IsRoot { get; set; }
    
    string ProcessorName { get; set; }
    IProcessor<TInputData>? ParentProcessor { get; set; }
    IProcessor<TInputData>? ProcessorFromDependentQue { get; set; }
    Task ProcessNextAsync(TInputData inputData);
    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
    event NotifyNestedProcessingCompleted NestedProcessingCompletedEvent;
    event NotifyCurrentProcessingCompleted CurrentProcessingCompletedEvent;
    event NotifyParentProcessingCompleted ParentProcessingCompletedEvent;
    delegate Task NotifyNestedProcessingCompleted();
    delegate Task NotifyCurrentProcessingCompleted(IProcessor<TInputData> processor);
    delegate Task NotifyParentProcessingCompleted(TInputData inputData);
    // IProcessor<TInput> GetNextProcessor()
}