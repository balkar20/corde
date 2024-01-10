using System.Threading.Tasks.Dataflow;
using TrafficControlApp.Config;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Contexts;
using TrafficControlApp.Models;
using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Consumers;

public class TrackConsumer : ITrackConsumer
{
    public SortedSet<int> Treads { get; set; }
    private IProcessor<Track> _rootProcessor;
    private TrafficProcessingContext _context;
    private ApplicationConfiguration _config;
    private Func<TrafficProcessingContext> ConfigureDependentProcessors;

    public TrackConsumer(ApplicationConfiguration config,  TrafficProcessingContext context, Func<TrafficProcessingContext> configureDependentProcessors)
    {
        _config = config;
        _context = context;
        this.ConfigureDependentProcessors = configureDependentProcessors;
    }
    
    public async Task ConsumeAllAsync(ISourceBlock<Track> buffer, Func<ITargetBlock<Track>, Task> startProducing)
    {
        _context.VehicleRootProcessor.NestedProcessingCompletedEvent += RootProcessorOnNestedProcessingCompleted;
        // _context.VehicleRootProcessor.CurrentProcessingCompleted += VehicleRootProcessorOnCurrentProcessingCompleted;
        var consumerBlock = new ActionBlock<Track>(
            track =>
            {
                var ctxt = GetFreshContext();
                
                //todo Wait for event before ProcessNext
                return  ctxt.VehicleRootProcessor.ProcessNextAsync(track);
            },
            new ExecutionDataflowBlockOptions 
            { BoundedCapacity = _config.BoundedCapacity,
                MaxDegreeOfParallelism = _config.MaxParallelConsumeCount });
        buffer.LinkTo(consumerBlock, new DataflowLinkOptions() 
            { PropagateCompletion = _config.PropagateCompletion });

        await startProducing(consumerBlock);

        await consumerBlock.Completion;
    }
    

    private TrafficProcessingContext GetFreshContext()
    {
        return _context;
    }

    private async Task RootProcessorOnNestedProcessingCompleted()
    {
        // _context.InitializeProcessors(applicationConfiguration, _mapper, new EventLoggingService(_logger));
        _context = ConfigureDependentProcessors();
        _context.VehicleRootProcessor.NestedProcessingCompletedEvent += RootProcessorOnNestedProcessingCompleted;
    }

    // private void ConfigureDependentProcessors()
    // {
    //     throw new NotImplementedException();
    // }
}