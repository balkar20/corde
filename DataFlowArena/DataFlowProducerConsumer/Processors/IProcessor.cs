using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Processors;

public interface IProcessor<in TInputData>
    // where TOutputData : new()
    // where TOutputData : IProcessResult
{
    // TOutputData ProcessAsync(TInputData inputData);
    bool CanRun { get; set; }
    bool Completed { get; set; }
    Task ProcessAsync(TInputData inputData);
}