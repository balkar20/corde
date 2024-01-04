using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions;

public class ProcessingItemResultCreationException: ProcessionResultException
{
    public ProcessingItemResultCreationException(IProcessionResult analysingItem) : base(analysingItem)
    {
    }
}