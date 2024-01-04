using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class ProcessingException: ApplicationException
{
    public ProcessingException(ProcessingItem analysingItem)
    {
        base.Data.Add(analysingItem.ItemId, analysingItem);
    }
    
}