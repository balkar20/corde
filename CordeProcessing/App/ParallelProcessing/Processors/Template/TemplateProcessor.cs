using ParallelProcessing.Models.Items.Base;
using ParallelProcessing.Models.Results.Procession.Abstractions;
using ParallelProcessing.Processors.Abstractions;
using ParallelProcessing.Services.Events.Abstractions;
using ParallelProcessing.Services.Events.Data.Enums;

namespace ParallelProcessing.Processors.Template;

public class TemplateProcessor<TInput, TProcessionResult>
    : ProgressiveProcessor<TInput, TProcessionResult>
    where TInput : ApplicationItem<string>
    where TProcessionResult : IProcessionResult
{
    private readonly Func<TInput, Task<TProcessionResult>> processLogic;
    public TemplateProcessor(
        IEventLoggingService? loggingService,
        string processorName,
        Func<TInput, Task<TProcessionResult>> processLogic)
        : base(loggingService, processorName)
    {
        this.processLogic = processLogic;
    }
    protected override async Task<IProcessionResult> ProcessLogic(TInput inputData)
    {
        var result =  await processLogic(inputData);
        await loggingService.Log($"{ProcessorName} + time {DateTime.Now}", EventLoggingTypes.ProcessedProcessor);
        return result;
    }

    protected override Task SetProcessionResult(TProcessionResult result)
    {
        return Task.CompletedTask;
    }
}