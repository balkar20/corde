namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInputData>
{
    public bool IsCompletedNestedProcessing { get; set; }
    public bool IsCompletedCurrentProcessing { get; set; }
    public bool IsStartedSelfProcessing { get; set; }
    string ProcessorName { get; set; }
    IProcessor<TInputData>? ParentProcessor { get; set; }
    Task ProcessNextAsync(TInputData inputData);
    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
    event NotifyNestedProcessingCompleted NestedProcessingCompletedEvent;
    event NotifyCurrentProcessingCompleted CurrentProcessingCompletedEvent;
    delegate Task NotifyNestedProcessingCompleted();
    delegate Task NotifyCurrentProcessingCompleted(IProcessor<TInputData> processor);
    // IProcessor<TInput> GetNextProcessor()
}