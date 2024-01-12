namespace TrafficControlApp.Services.Events.Data.Enums;

public enum EventLoggingTypes
{
    ProcessedEvent,
    CallMethodInProcessor,
    CallMethodInProcessorWithCondition,
    CallMethodInProcessorWithCompletedDependant,
    HandlingEvent,
    RaisingEvent,
    ProcessionInformation,
    SubscribingToEvent,
    ThreadIdLogging,
    ExceptionKindEvent
}