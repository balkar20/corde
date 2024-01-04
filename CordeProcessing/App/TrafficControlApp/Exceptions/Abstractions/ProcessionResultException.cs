using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class ProcessionResultException: ApplicationException
{
    public ProcessionResultException(IProcessionResult analysingItem)
    {
        base.Data.Add(analysingItem.IsSucceed, analysingItem);
    }
    
}