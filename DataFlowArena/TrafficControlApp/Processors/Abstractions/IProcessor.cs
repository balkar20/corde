namespace TrafficControlApp.Processors.Abstractions;

public interface IProcessor<in TInputData>
    // where TOutputData : new()
    // where TOutputData : IProcessResult
{
    // TOutputData ProcessAsync(TInputData inputData);
    bool CanRun { get; set; }
    
    string ProcessId { get; set; }
    
    string InputId { get; set; }
    // bool CompletedWithDependentProcessors { get; set; }
    Task ProcessNextAsync(TInputData inputData);
}