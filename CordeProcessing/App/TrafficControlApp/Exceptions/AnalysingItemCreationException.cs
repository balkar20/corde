using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Items.Analysing;

namespace TrafficControlApp.Exceptions;

public class AnalysingItemCreationException: AnalysingException
{
    public AnalysingItemCreationException(AnalysingItem analysingItem) : base(analysingItem)
    {
    }
}