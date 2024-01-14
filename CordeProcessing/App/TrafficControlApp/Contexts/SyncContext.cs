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

    public SyncContext(IEventLoggingService? loggingService)
    {
        LoggingService = loggingService;
    }

    public async Task WaitLockWithCallback(IProcessor<TInput> processor, Func< TInput, Task> callback, TInput input)
    {
        var (executionName, acquireId) = await AcquireAndLog(processor);
        
        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);

        var isNeedToKeepLock = !processor.IsStartedSelfProcessing &&
                               processor.IsRoot &&
                               dependentProcessorExists;
        bool isNeedToKeepLockedUntilDependentRootReleased =
            processor.IsStartedSelfProcessing &&
            processor.IsCompletedCurrentProcessing &&
            dependentProcessorExists &&
            dependantProcessor.IsRoot &&
            !dependantProcessor.IsStartedSelfProcessing;

        if (dependantProcessor == null &&
            processor.RootProcessorFromDependentQueue?.DependedProcessors.Count > 0 &&
            processor.RootProcessorFromDependentQueue.IsCompletedCurrentProcessing &&
            !processor.RootProcessorFromDependentQueue.IsCompletedNestedProcessing)
        {
            processor.DependentProcessorsExecutingCount =
                processor.RootProcessorFromDependentQueue.DependentProcessorsExecutingCount;
        }
        try
        {
            if (processor.DependentProcessorsExecutingCount > 0 && !isNeedToKeepLockedUntilDependentRootReleased)
            {
                processor.DependentProcessorsExecutingCount -= 1;
                await LogAndRelease(acquireId, processor, executionName, true);
            }
            await callback(input);   
        }
        finally
        {
            if (isNeedToKeepLock || (!isNeedToKeepLock && isNeedToKeepLockedUntilDependentRootReleased))
            {
                await LogAndRelease(acquireId, processor, executionName);
            }
        }
    }

    private async Task<(string, int)> AcquireAndLog(IProcessor<TInput> processor)
    {
        await SemaphoreSlim.WaitAsync();

        var acquireId = new Random().Next(1, 10000);
        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);
        var rootCompleted = processor.IsStartedSelfProcessing && processor.IsCompletedCurrentProcessing;
        var dependantExistsAndRootCompleted = dependentProcessorExists && rootCompleted;

        var dependantProcessorTypeName =
            dependantExistsAndRootCompleted ? dependantProcessor.ProcessorTypeName : HimselfLabel;
        
        await LoggingService.Log($"Id = {acquireId} AcquireTime{DateTime.Now}, Name = {processor.ProcessorName}, TypeName = {processor.ProcessorTypeName} for {dependantProcessorTypeName}",
            EventLoggingTypes.SemaphoreAcquired, processor.RootProcessorFromDependentQueue?.ProcessorTypeName);
        return (dependantProcessorTypeName, acquireId);
    }
    
    private async Task LogAndRelease(int acquireId, IProcessor<TInput> processor, string executionName, bool isExecutesAfterRelease = false)
    {
        var dependentProcessorExists = processor.DependedProcessors.TryPeek(out var dependantProcessor);
        var bufList =  processor.DependedProcessors.ToList();
        var dependantNames = GetElementNames(bufList);
        var executingAfterReleaseMessage = isExecutesAfterRelease ? $"Executing After Release With execution Dependencies: {dependantNames}" : "";
        var rootCompleted = processor.IsStartedSelfProcessing && processor.IsCompletedCurrentProcessing;
        var dependantExistsAndRootCompleted = dependentProcessorExists && rootCompleted;
        var dependantExistsAndRootCompletedAndDependantNotCompleted = dependantExistsAndRootCompleted 
                                                                      && !dependantProcessor.IsCompletedCurrentProcessing;
        
        var dependantProcessorTypeName =
            dependantExistsAndRootCompleted ? dependantProcessor.ProcessorTypeName : HimselfLabel;
        if (dependantExistsAndRootCompletedAndDependantNotCompleted)
        {
            dependantProcessorTypeName = HimselfLabel;
        }
        await LoggingService.Log($"Id = {acquireId}, ReleaseTime{DateTime.Now}, Name = {processor.ProcessorName}, TypeName = {processor.ProcessorTypeName} , for {(dependentProcessorExists ? 
            dependantProcessorTypeName : 
            HimselfLabel)} {executingAfterReleaseMessage}" , EventLoggingTypes.SemaphoreReleased, executionName); 

        SemaphoreSlim.Release();
    }
    
    private string GetElementNames(List<IProcessor<TInput>> list)
    {
        return string.Join(", ", list.Select(obj => obj.ProcessorTypeName));
    }
}