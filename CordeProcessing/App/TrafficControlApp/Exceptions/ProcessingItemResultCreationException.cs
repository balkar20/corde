using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Results.Procession.Abstractions;

namespace TrafficControlApp.Exceptions;

public class ProcessingItemResultCreationException: ProcessionResultException
{
    public ProcessingItemResultCreationException(IProcessionResult analysingItem) : base(analysingItem)
    {
    }
}