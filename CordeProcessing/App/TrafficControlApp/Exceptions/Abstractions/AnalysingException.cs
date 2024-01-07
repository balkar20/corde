using TrafficControlApp.Models.Items.Analysing;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class AnalysingException: ApplicationException
{
    public AnalysingException(AnalysingItem analysingItem)
    {
        base.Data.Add(analysingItem.ItemId, analysingItem);
    }
    
}