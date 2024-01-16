using System.Collections.Concurrent;
using AutoMapper;
using Serilog;
using TrafficControlApp.Config;
using TrafficControlApp.Contexts;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Processors.Template;
using TrafficControlApp.Services.Events.Services;

namespace TrafficControlApp.Root;

public class TestCasesConfiguration
{
    public  TrafficProcessingContext ConfigureDependentProcessorsCase1(ApplicationConfiguration applicationConfiguration,ILogger logger,IMapper mapper)
    {
        var _context = new TrafficProcessingContext(applicationConfiguration);
        
        var loggerService = new EventLoggingService(logger);

        _context.InitializeProcessors(applicationConfiguration, mapper, loggerService);
        
        //TypeDependant
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleMarkProcessor);
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleDangerProcessor);
        
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleColorProcessor);
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleTrafficProcessor);
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleSeasonProcessor);
        
        var newProc = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateFirstProcessor", _context.GetLongRunningTask);
        var newProc2 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateSecondProcessor", _context.GetLongRunningTask);
        var newProc3 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateThirdProcessor", _context.GetLongRunningTask);
        var newProc4 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateFourProcessor", _context.GetLongRunningTask);
        _context.VehicleSeasonProcessor.AddDependentProcessor(newProc);
        _context.VehicleSeasonProcessor.AddDependentProcessor(newProc2);
        _context.VehicleSeasonProcessor.AddDependentProcessor(newProc3);
        

        applicationConfiguration.MaxParallelConsumeCount = _context.VehicleRootProcessor.TotalAmountOfProcessors;
        return _context;
    }
    
    public  TrafficProcessingContext  ConfigureDependentProcessorsCase2(ApplicationConfiguration applicationConfiguration,ILogger logger,IMapper mapper)
    {
        var _context = new TrafficProcessingContext(applicationConfiguration);
        
        var loggerService = new EventLoggingService(logger);

        _context.InitializeProcessors(applicationConfiguration, mapper, loggerService);
        
        //TypeDependant
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleMarkProcessor);
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleDangerProcessor);
        
        
        //MarkDependant                                  
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleColorProcessor);
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleSeasonProcessor);
        // _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleTrafficProcessor);                                
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleColorProcessor);
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleSeasonProcessor);
        _context.VehicleDangerProcessor.AddDependentProcessor(_context.VehicleTrafficProcessor);

        var newProc = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateFirstProcessor", _context.GetLongRunningTask);
        var newProc2 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateSecondProcessor", _context.GetLongRunningTask);
        var newProc3 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateThirdProcessor", _context.GetLongRunningTask);
        var newProc4 = new TemplateProcessor<Track, PoolProcessionResult>(loggerService, "TemplateFourProcessor", _context.GetLongRunningTask);
        _context.VehicleTrafficProcessor.AddDependentProcessor(newProc);
        _context.VehicleTrafficProcessor.AddDependentProcessor(newProc2);
        _context.VehicleTrafficProcessor.AddDependentProcessor(newProc3);
        // _context.VehicleDangerProcessor.AddDependentProcessor(newProc2);
        // _context.VehicleDangerProcessor.AddDependentProcessor(newProc3);
        // _context.VehicleDangerProcessor.AddDependentProcessor(newProc4);
        string[] newNames = {"Volvo", "BMW", "Ford"};
        string[] newNames2 = {"Koko", "Mila", "Oni"};
        var deps = GetTemplateProcessorsWithNames(newNames, loggerService, _context);
        var dep2 = GetTemplateProcessorsWithNames(newNames2, loggerService, _context);
        var depsQueue = new ConcurrentQueue<IProcessor<Track>>(deps);
        var depsQueue2 = new ConcurrentQueue<IProcessor<Track>>(dep2);
        newProc2.SetDependents(depsQueue);
        newProc3.SetDependents(depsQueue2);

        applicationConfiguration.MaxParallelConsumeCount = _context.VehicleRootProcessor.TotalAmountOfProcessors;
        return _context;
    }
    
    
    private List<TemplateProcessor<Track, PoolProcessionResult>> GetTemplateProcessorsWithNames(string[] names, EventLoggingService eventLoggingService, TrafficProcessingContext context)
    {
        return new List<TemplateProcessor<Track, PoolProcessionResult>>(names.Select(n =>
            new TemplateProcessor<Track, PoolProcessionResult>(eventLoggingService, n, context.GetLongRunningTask)));
    }
}