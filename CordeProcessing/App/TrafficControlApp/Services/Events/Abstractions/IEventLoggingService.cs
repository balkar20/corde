using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Services.Events.Abstractions;

public interface IEventLoggingService
{
    Task LogEvent(string eventDataString, EventLoggingTypes type, string additional = "");
}