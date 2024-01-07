using TrafficControlApp.Contexts;
using TrafficControlApp.Processors;
using TrafficControlApp.Root;
using TrafficControlApp.Services;

namespace TrafficControlApp.Tests.StartUp;

public class ProcessorsTests
{
    [Fact]
    public  async Task ConfigureDependentProcessors()
    {
        var startUpConfigurator = new TrafficControlStartupConfigurator();
        startUpConfigurator.Configure();
        
        await startUpConfigurator.Test();
    
        // var _context = new TrafficProcessingContext(new SharedMemoryStorage(), applicationConfiguration);
        
        // _context.InitializeProcessors(applicationConfiguration, _mapper, new EventLoggingService(_logger));new vehicle

        // var root = new VehicleTypeProcessor();
        //TypeDependant
        // _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleMarkProcessor);
        // _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleDangerProcessor);
        //     
        // //MarkDependant                                   
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleColorProcessor);
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleSeasonProcessor);
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleTrafficProcessor);
    }
}