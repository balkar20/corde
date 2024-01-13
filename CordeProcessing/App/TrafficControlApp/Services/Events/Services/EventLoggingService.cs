using Serilog;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Services.Events.Services;

public class EventLoggingService: IEventLoggingService
{
    private readonly ILogger _logger;

    public EventLoggingService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task Log(string eventDataString, EventLoggingTypes type, string additional = "")
    {
        var message = type switch
        {
            EventLoggingTypes.ProcessedEvent => $"ProcessedEvent Message with String :{eventDataString}",
            EventLoggingTypes.CallMethodInProcessor => $"CallMethodInProcessor Message with String :{eventDataString}",
            EventLoggingTypes.CallMethodInProcessorWithCondition => $"CallMethodInProcessorWithCondition Message with String :{eventDataString} and condition:{additional}",
            EventLoggingTypes.CallMethodInProcessorWithCompletedDependant => $"CallMethodInProcessorWithCompletedDependant  with thisName :{eventDataString} and DeperndantName:{additional}",
            EventLoggingTypes.HandlingEvent => $"HandlingEvent Message with String Of  :{eventDataString}, processor called = {additional}",
            EventLoggingTypes.RaisingEvent => $"RaisingEvent Message with String :{eventDataString}",
            EventLoggingTypes.ProcessionInformation => $"ProcessionInformation Message with String :{eventDataString}",
            EventLoggingTypes.SubscribingToEvent => $"SubscribingToEvent  with Name :{eventDataString} and ProcessorName: {additional}",
            EventLoggingTypes.ThreadIdLogging => $"ThreadId :{eventDataString} for Processor : {additional}",
            EventLoggingTypes.ExceptionKindEvent => $"ExceptionKindEvent :{eventDataString} for Processor : {additional}",
            EventLoggingTypes.SemaphoreAcquired => $"SemaphoreAcquired for Processor:{eventDataString}",
            EventLoggingTypes.SemaphoreReleased => $"SemaphoreReleased for Processor:{eventDataString}, Execution name: {additional}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        _logger.Information(message);
    }
}