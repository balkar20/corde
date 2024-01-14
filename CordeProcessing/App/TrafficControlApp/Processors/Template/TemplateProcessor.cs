using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Processors.Template;

public class TemplateProcessor<TInput, TProcessionResult>(
    IEventLoggingService? loggingService,
    string processorName,
    Func<TInput, Task<TProcessionResult>> processLogic)
    : Processor<TInput, TProcessionResult>(loggingService, processorName)
    where TInput : ApplicationItem<string>
    where TProcessionResult : IProcessionResult
{
    protected override async Task<IProcessionResult> ProcessLogic(TInput inputData)
    {
        return await processLogic(inputData);
    }

    protected override Task SetProcessionResult(TProcessionResult result)
    {
        return Task.CompletedTask;
    }
}