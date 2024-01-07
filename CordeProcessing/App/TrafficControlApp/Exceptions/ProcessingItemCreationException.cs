using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Items.Processing;

namespace TrafficControlApp.Exceptions;

public class ProcessingItemCreationException: ProcessingException
{
    public ProcessingItemCreationException(ProcessingItem analysingItem) : base(analysingItem)
    {
    }
}