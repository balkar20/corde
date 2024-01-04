using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class AnalysingException: ApplicationException
{
    public AnalysingException(AnalysingItem analysingItem)
    {
        base.Data.Add(analysingItem.ItemId, analysingItem);
    }
    
}