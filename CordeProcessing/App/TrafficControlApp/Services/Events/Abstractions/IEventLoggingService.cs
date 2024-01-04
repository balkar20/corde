namespace TrafficControlApp.Services.Events.Abstractions;

public interface IEventLoggingService
{
    Task LogEvent(string eventData);
}