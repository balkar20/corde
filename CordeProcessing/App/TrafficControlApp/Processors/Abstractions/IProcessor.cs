namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInputData>
{
    public bool CompletedProcessing { get; set; }
    Task ProcessNextAsync(TInputData inputData);
    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
    event NotifyNestedProcessingCompleted NestedProcessingCompleted;
    delegate void NotifyNestedProcessingCompleted();
    // IProcessor<TInput> GetNextProcessor()
}