using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions;

public class AnalysingItemCreationException: AnalysingException
{
    public AnalysingItemCreationException(AnalysingItem analysingItem) : base(analysingItem)
    {
    }
}