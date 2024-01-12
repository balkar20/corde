using TrafficControlApp.Services.Events.Data.Enums;

namespace TrafficControlApp.Services.Events.Abstractions;

public interface IEventLoggingService
{
    Task Log(string eventDataString, EventLoggingTypes type, string additional = "");
}