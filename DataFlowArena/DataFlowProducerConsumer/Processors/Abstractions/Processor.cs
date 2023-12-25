using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Processors.Abstractions;

public abstract class Processor<TInput, TOutput> : IProcessor<TInput, TOutput> 
    where TOutput : IProcessResult, new()
{
    public bool CanRun { get; set; }
    public bool Completed { get; set; }
    public async ValueTask<TOutput> ProcessAsync(TInput inputData)
    {
        if (CanRun)
        {

            return await ProcessLogic(inputData);
        }
        
        
        return new()
        {
            IsSucceed = false
        };
    }

    public abstract Task ProcessLogic(TInput inputData);
}