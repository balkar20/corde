using TrafficControlApp.Models.Results.Procession.Abstractions;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class ProcessionResultException: ApplicationException
{
    public ProcessionResultException(IProcessionResult analysingItem)
    {
        base.Data.Add(analysingItem.IsSucceed, analysingItem);
    }
    
}