using ParallelProcessing.Services.Events.Data.Enums;

namespace ParallelProcessing.Services.Events.Abstractions;

public interface IEventLoggingService
{
    Task Log(string eventDataString, EventLoggingTypes type, string additional = "");
}