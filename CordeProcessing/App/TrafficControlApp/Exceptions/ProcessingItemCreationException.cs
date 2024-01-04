using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions;

public class ProcessingItemCreationException: ProcessingException
{
    public ProcessingItemCreationException(ProcessingItem analysingItem) : base(analysingItem)
    {
    }
}