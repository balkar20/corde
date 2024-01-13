using System.Collections.Concurrent;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Contexts;

public class SyncContext<TInput>
{
    private readonly IEventLoggingService? LoggingService;
    
    const string HimselfLabel = "Himself";
    public SemaphoreSlim SemaphoreSlim { get; set; } = new (1, 1);
    // public SemaphoreSlim SemaphoreSlim2 { get; set; }= new (1, 2);
    // public SemaphoreSlim SemaphoreSlim3 { get; set; }= new (1, 3);
    public object lockObj = new ();
    private int  oo  = 1;
    public ConcurrentBag<object> SyncList { get; set; }
    public int count;

    public int Count
    {
        get
        {
            return count;
        }
    }

    public SyncContext(IEventLoggingService? loggingService)
    {
        LoggingService = loggingService;
    }

    public async Task<bool> WaitLock(IProcessor<TInput> processor)
    {
        await SemaphoreSlim.WaitAsync();
                  
        // if (processor.ProcessorsDepended.TryPeek( out var processorDependant)
        //     && processorDependant.IsRoot)
        // {
        //     return false;cr
        var pec = processor.DependentProcessorsExecutingCount;
        // Interlocked.Increment(ref oo);
        if (processor.DependentProcessorsExecutingCount > 0)
        {
            processor.DependentProcessorsExecutingCount -= 1;

            SemaphoreSlim.Release();
            return true;
        }
        count++;

        return false;
    }

    public async Task WaitLockWithCallback(IProcessor<TInput> processor, Func< TInput, Task> callback, TInput input)
    {
        var executionName = await AcquireAndLog(processor);
        
        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);
        
        bool isNeedToKeepLockedUntilDependentRootReleased =
            processor.IsStartedSelfProcessing &&
            processor.IsCompletedCurrentProcessing &&
            dependentProcessorExists &&
            dependantProcessor.IsRoot &&
            !dependantProcessor.IsStartedSelfProcessing;
        if (isNeedToKeepLockedUntilDependentRootReleased)
        {
            processor.CurrentProcessingCompletedEvent += async o => await ProcessorOnCurrentProcessingCompletedEvent(o);
        }
        
        try
        {
            if (processor.DependentProcessorsExecutingCount > 0 && !isNeedToKeepLockedUntilDependentRootReleased)
            {
                processor.DependentProcessorsExecutingCount -= 1;

                await LogAndRelease(processor, executionName);
            }
            await callback(input);   
        }
        finally
        {
            count++;
            if (!isNeedToKeepLockedUntilDependentRootReleased)
            {
                
                await LogAndRelease(processor, executionName);
            }
        }
    }

    private async Task ProcessorOnCurrentProcessingCompletedEvent(TInput inputdata)
    {
        SemaphoreSlim.Release();
    }

    private async Task ProcessorOnCurrentProcessingCompletedEvent(IProcessor<TInput> processor)
    {
        LogAndRelease(processor, processor.ProcessorTypeName);
    }

    private async Task LogAndRelease(IProcessor<TInput> processor, string executionName)
    {
        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);
        await LoggingService.Log($"{processor.ProcessorTypeName} for {(dependentProcessorExists ? dependantProcessor.ProcessorTypeName : HimselfLabel)}" , EventLoggingTypes.SemaphoreReleased, executionName); 

        SemaphoreSlim.Release();
    }

    private async Task<string> AcquireAndLog(IProcessor<TInput> processor)
    {
        await SemaphoreSlim.WaitAsync();

        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);
        var rootCompleted = processor.IsStartedSelfProcessing && processor.IsCompletedCurrentProcessing;
        var dependantExistsAndRootCompleted = dependentProcessorExists && rootCompleted;

        var dependantProcessorTypeName =
            dependantExistsAndRootCompleted ? dependantProcessor.ProcessorTypeName : HimselfLabel;
        await LoggingService.Log($"{processor.ProcessorTypeName} for {dependantProcessorTypeName}",
            EventLoggingTypes.SemaphoreAcquired);
        return dependantProcessorTypeName;
    }
}