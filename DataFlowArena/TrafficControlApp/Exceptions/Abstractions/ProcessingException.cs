using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Exceptions.Abstractions;

public abstract class ProcessingException<TItem>: ApplicationException
{
    public ProcessingException(Processor<TItem> processor)
    {
        base.Data.Add(processor.ProcessorId, processor);
    }
    
}