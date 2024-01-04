using Serilog;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Services.Events.Services;

public class EventLoggingService: IEventLoggingService
{
    private readonly ILogger _logger;

    public EventLoggingService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task LogEvent(string eventData)
    {
        _logger.Information($"!!!!!!!!!!!!!!!Mock Implementation of Event Handling!!!!!!!!!!!!!:Event data:/n{eventData}");
    }
}