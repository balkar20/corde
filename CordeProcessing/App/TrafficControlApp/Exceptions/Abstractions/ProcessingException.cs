using TrafficControlApp.Models.Items.Processing;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class ProcessingException: ApplicationException
{
    public ProcessingException(ProcessingItem analysingItem)
    {
        base.Data.Add(analysingItem.ItemId, analysingItem);
    }
    
}