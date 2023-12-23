using System.Threading.Tasks.Dataflow;

namespace DataFlowProducerConsumer.Processors;

public interface IProcessor<in TInputData,TOutputData>
{
    // TOutputData ProcessAsync(TInputData inputData);
    Task<TOutputData> ProcessAsync(ISourceBlock<TOutputData> source);
}