using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Processors;

public interface IProcessor<in TInputData, TOutputData>
    // where TOutputData : new()
    where TOutputData : IProcessResult
{
    // TOutputData ProcessAsync(TInputData inputData);
    bool CanRun { get; set; }
    bool Completed { get; set; }
    ValueTask<TOutputData> ProcessAsync(TInputData inputData);
}