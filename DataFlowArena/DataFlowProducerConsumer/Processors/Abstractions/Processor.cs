using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Processors.Abstractions;

public abstract class Processor<TInput> : IProcessor<TInput> 
    // where TOutput : IProcessResult, new()
{
    public bool CanRun { get; set; }
    
    public bool Completed { get; set; }
    
    public bool FullyCompleted { get; set; }
    
    public Queue<Processor<TInput>?> ProcessorsDepended { get; set; }
    
    public Processor<TInput> NextProcessor { get; set; }
    
    public async Task ProcessAsync(TInput inputData)
    {
        if (Completed)
        {
            await RunNextProcessor(inputData);
            return;
        }

        await ProcessLogic(inputData);
        Completed = true;
    }

    public abstract Task<IProcessResult> ProcessLogic(TInput inputData);

    private async Task RunNextProcessor(TInput inputData)
    {
        ProcessorsDepended.TryDequeue(out Processor<TInput>? proc);
        if (proc != null)
        {
            await proc?.ProcessAsync(inputData);
        }
        
    }

    public void AddDependentProcessor(Processor<TInput> dependentProcessor)
    {
        // 1. Some validation for processor
        ProcessorsDepended.Enqueue(dependentProcessor);
    }
}