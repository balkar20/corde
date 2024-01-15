using System.Collections.Concurrent;
using AutoMapper;
using TrafficControlApp.Config;
using TrafficControlApp.Consumers;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Contexts;
using TrafficControlApp.Producers;
using TrafficControlApp.Producers.Abstraction;
using TrafficControlApp.Root.Abstractions;
using Microsoft.Extensions.Configuration;
using Serilog;
using TrafficControlApp.ClientDevices.Abstractions;
using TrafficControlApp.ClientDevices.Devices;
using TrafficControlApp.Mapping;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Processors.Template;
using TrafficControlApp.Services.Events.Services;


namespace TrafficControlApp.Root;

public class TrafficControlStartupConfigurator : StartupConfigurator
{
    private ApplicationConfiguration applicationConfiguration;
    private TrafficProcessingContext _context;
    
    private ITrackConsumer _trackConsumer;
    private ITrackProducer _trackProducer;
    private IMapper _mapper;
    private ITrackDevice _trackDevice;
    private ILogger _logger;

    public TrafficControlStartupConfigurator()
    {
        _trackDevice = new TrackDevice();
        Configure();
    }
    
    public override async Task Run()
    {
        // _context.VehicleRootProcessor.NestedProcessingCompletedEvent += RootProcessorOnNestedProcessingCompleted;
        await new TrafficFlowProcessStarter(
                _trackDevice, 
                _trackProducer,
                _trackConsumer, 
                applicationConfiguration)
            .StartProcess();
    }
    
    public  async Task Test()
    {
        var bunch =  await _trackDevice.GiveMeTrackDataBunch("Type", applicationConfiguration.MaxParallelConsumeCount);
        // _context.VehicleRootProcessor.NestedProcessingCompletedEvent += RootProcessorOnNestedProcessingCompleted;
        foreach (var bunchTrack in bunch.Tracks)
        {
            await _context.VehicleRootProcessor.ProcessNextAsync(bunchTrack);
        }
    }

    protected override void CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false);
        // .(Directory.GetCurrentDirectory())
        // .AddJsonFile("config.json", optional: false);

        IConfiguration config = builder.Build();
        //Create AppConfiguration
        //todo Make it deserialized from IConfiguration
        applicationConfiguration = new ApplicationConfiguration()
        {
            BoundedCapacity = 100,
            ProduceSpeed = TimeSpan.FromSeconds(0.5),
            MaxParallelConsumeCount = 6,
            ConsumeSpeed = TimeSpan.FromSeconds(2),
            PropagateCompletion = true,
            VehicleTypeAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleColorAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleSeasonAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleTrafficAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleDangerAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(3),
                // TimeForAnalyse = TimeSpan.FromSeconds(30),
            },
            VehicleMarkAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(3),
                // TimeForAnalyse = TimeSpan.FromSeconds(30),
            }
        };
    }

    protected override void ConfigureProducers()
    {
        _trackProducer = new TrackProducer(_trackDevice, applicationConfiguration);
    }

    protected override void ConfigureConsumers()
    {
        _trackConsumer = new TrackConsumer(applicationConfiguration, _context, ConfigureDependentProcessors);
    }

    protected override void ConfigureMapping()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TrackProcessingMappingProfile());  //mapping between Web and Business layer objects// mapping between Business and DB layer objects
        });
        _mapper = config.CreateMapper();
    }

    protected override TrafficProcessingContext ConfigureDependentProcessors()
    {
        _context = new TrafficProcessingContext(applicationConfiguration);
        
        var loggerService = new EventLoggingService(_logger);

        _context.InitializeProcessors(applicationConfiguration, _mapper, loggerService);
        
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
        var deps = GetTemplateProcessorsWithNames(newNames, loggerService, _context);
        var depsQueue = new ConcurrentQueue<IProcessor<Track>>(deps);
        newProc3.SetDependents(depsQueue);

        applicationConfiguration.MaxParallelConsumeCount = _context.VehicleRootProcessor.TotalAmountOfProcessors;
        return _context;
    }

    private List<TemplateProcessor<Track, PoolProcessionResult>> GetTemplateProcessorsWithNames(string[] names, EventLoggingService eventLoggingService, TrafficProcessingContext context)
    {
        return new List<TemplateProcessor<Track, PoolProcessionResult>>(names.Select(n =>
            new TemplateProcessor<Track, PoolProcessionResult>(eventLoggingService, n, context.GetLongRunningTask)));
    }
    
    protected override void ConfigureLogging()
    {
        // var credentials = new GrafanaLokiCredentials()
        // {
        //     User = "admin",
        //     Password = "admin"
        // };
        // _logger = new Serilog.Configuration.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ALabel", "ALabelValue")
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Hour)
            .WriteTo.Console()
            .CreateLogger();
            // .WriteTo.GrafanaLoki(
            //     "http://localhost:3100",
            //     credentials,
            //     new Dictionary<string, string>() { { "app", "Serilog.Sinks.GrafanaLoki.ProductWebApi" } }, // Global labels
            //     Serilog.Events.LogEventLevel.Debug
            // )
            // .CreateLogger();
        _logger = Log.Logger;
    }

    
}