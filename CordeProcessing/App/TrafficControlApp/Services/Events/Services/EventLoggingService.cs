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

    public async Task LogEvent(string eventDataString, EventLoggingTypes type, string additional = "")
    {
        var message = type switch
        {
            EventLoggingTypes.ProcessedEvent => $"ProcessedEvent Message with String :{eventDataString}",
            EventLoggingTypes.CallMethodInProcessor => $"CallMethodInProcessor Message with String :{eventDataString}",
            EventLoggingTypes.CallMethodInProcessorWithCondition => $"CallMethodInProcessorWithCondition Message with String :{eventDataString} and condition:{additional}",
            EventLoggingTypes.CallMethodInProcessorWithCompletedDependant => $"CallMethodInProcessorWithCompletedDependant  with thisName :{eventDataString} and DeperndantName:{additional}",
            EventLoggingTypes.HandlingEvent => $"HandlingEvent Message with String :{eventDataString}",
            EventLoggingTypes.RaisingEvent => $"RaisingEvent Message with String :{eventDataString}",
            EventLoggingTypes.ProcessionInformation => $"ProcessionInformation Message with String :{eventDataString}",
            EventLoggingTypes.SubscribingToEvent => $"SubscribingToEvent  with Name :{eventDataString} and ProcessorName: {additional}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        _logger.Information(message);
    }
}