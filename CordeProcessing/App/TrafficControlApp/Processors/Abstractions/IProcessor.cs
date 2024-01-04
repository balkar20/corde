namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<TInputData>
    // where TOutputData : new()
    // where TOutputData : IProcessResult
{
    // TOutputData ProcessAsync(TInputData inputData);
    // bool CanRun { get; set; }
    //                                 
    //                                 string ProcessId { get; set; }
    //                                 
    //                                 string InputId { get; set; }
    // bool CompletedWithDependentProcessors { get; set; }
    Task ProcessNextAsync(TInputData inputData);

    void AddDependentProcessor(IProcessor<TInputData> dependentProcessor);
}