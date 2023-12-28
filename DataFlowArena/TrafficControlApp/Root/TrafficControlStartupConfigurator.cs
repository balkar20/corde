using AutoMapper;
using TrafficControlApp.Config;
using TrafficControlApp.Consumers;
using TrafficControlApp.Consumers.Abstractions;
using TrafficControlApp.Contexts;
using TrafficControlApp.Producers;
using TrafficControlApp.Producers.Abstraction;
using TrafficControlApp.Root.Abstractions;
using TrafficControlApp.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using TrafficControlApp.ClientDevices.Abstractions;
using TrafficControlApp.ClientDevices.Devices;
using TrafficControlApp.Mapping;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Services.Analysers;


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
        await new TrafficFlowProcessStarter(
                _trackDevice, 
                _context.VehicleRootProcessor, 
                _trackProducer,
                _trackConsumer, applicationConfiguration)
            .StartProcess();
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
            MaxParallelConsumeCount = 4,
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
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleMarkAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            }
        };
    }

    protected override void ConfigureProducers()
    {
        _trackProducer = new TrackProducer(_trackDevice, applicationConfiguration);
    }

    protected override void ConfigureConsumers()
    {
        _trackConsumer = new TrackConsumer(applicationConfiguration, _context.VehicleRootProcessor);
    }

    protected override void ConfigureMapping()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TrackProcessingMappingProfile());  //mapping between Web and Business layer objects// mapping between Business and DB layer objects
        });
        _mapper = config.CreateMapper();
    }

    protected override void ConfigureDependentProcessors()
    {
        _context = new TrafficProcessingContext(new SharedMemoryStorage(), applicationConfiguration);
        _context.InitializeProcessors(applicationConfiguration, _mapper);
        
        //TypeDependant
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleMarkProcessor);
        _context.VehicleRootProcessor.AddDependentProcessor(_context.VehicleDangerProcessor);
        
        //MarkDependant                                   
        _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleColorProcessor);
        _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleSeasonProcessor);
        _context.VehicleMarkProcessor.AddDependentProcessor(_context.VehicleTrafficProcessor);
    }
    
    public void ConfigureLogging()
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