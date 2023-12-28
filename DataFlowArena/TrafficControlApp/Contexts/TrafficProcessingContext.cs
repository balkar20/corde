using AutoMapper;
using Serilog;
using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage;

namespace TrafficControlApp.Contexts;

public class TrafficProcessingContext
{
    private readonly ISharedMemoryVehicleService _sharedMemoryVehicleService;
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IEventLoggingService _logger;

    public TrafficProcessingContext(ISharedMemoryVehicleService sharedMemoryVehicleService, ApplicationConfiguration applicationConfiguration)
    {
        _sharedMemoryVehicleService = sharedMemoryVehicleService;
        _applicationConfiguration = applicationConfiguration;
    }

    #region Processors

    // public Processor<Track, TypeAnalyseResult> VehicleRootProcessor { get; set; }
    // public Processor<Track, MarkAnalyseResult> VehicleMarkProcessor { get; set; }
    // public Processor<Track, ColorAnalyseResult> VehicleColorProcessor { get; set; }
    // public Processor<Track, SeasonAnalyseResult> VehicleSeasonProcessor { get; set; }
    // public Processor<Track, DangerAnalyseResult> VehicleDangerProcessor { get; set; }
    // public Processor<Track, TrafficAnalyseResult> VehicleTrafficProcessor { get; set; }

    public IProcessor<Track> VehicleRootProcessor { get; set; }
    public IProcessor<Track> VehicleMarkProcessor { get; set; }
    public IProcessor<Track> VehicleColorProcessor { get; set; }
    public IProcessor<Track> VehicleSeasonProcessor { get; set; }
    public IProcessor<Track> VehicleDangerProcessor { get; set; }
    public IProcessor<Track> VehicleTrafficProcessor { get; set; }

    #endregion

    #region Public Methods

    public void InitializeProcessors(ApplicationConfiguration configuration, IMapper mapper)
    {
        var analysers = GetAnalysers();
        VehicleRootProcessor = new VehicleTypeProcessor(_sharedMemoryVehicleService, analysers.vehicleTypeAnalyzerService, mapper, _logger);
        VehicleSeasonProcessor = new VehicleSeasonProcessor(_sharedMemoryVehicleService, analysers.seasonAnalyzerService, mapper, _logger);
        VehicleColorProcessor = new VehicleColorProcessor(_sharedMemoryVehicleService, analysers.colorAnalyzerService, mapper, _logger);
        VehicleMarkProcessor = new VehicleMarkProcessor(_sharedMemoryVehicleService, analysers.markAnalyzerService, mapper, _logger);
        VehicleTrafficProcessor = new VehicleTrafficProcessor(_sharedMemoryVehicleService, analysers.trafficAnalyzerService, mapper, _logger);
        VehicleDangerProcessor = new VehicleDangerProcessor(_sharedMemoryVehicleService, analysers.dangerAnalyzerService, mapper, _logger);
    }
    
    private (
        IVehicleAnalyzerService<IAnalysingResult> vehicleTypeAnalyzerService,
        IVehicleAnalyzerService<IAnalysingResult> colorAnalyzerService,
        IVehicleAnalyzerService<IAnalysingResult> seasonAnalyzerService,
        IVehicleAnalyzerService<IAnalysingResult> markAnalyzerService,
        IVehicleAnalyzerService<IAnalysingResult> trafficAnalyzerService,
        IVehicleAnalyzerService<IAnalysingResult> dangerAnalyzerService
        ) GetAnalysers()
    {
        var aso = new VehicleTypeAnalyzerService(_applicationConfiguration.VehicleTypeAnalyseConfig);
        var asi = new VehicleColorAnalyzerService(_applicationConfiguration.VehicleColorAnalyseConfig);
        return (
            new VehicleTypeAnalyzerService(_applicationConfiguration.VehicleTypeAnalyseConfig),
            new VehicleColorAnalyzerService(_applicationConfiguration.VehicleColorAnalyseConfig),
            new VehicleSeasonAnalyzerService(_applicationConfiguration.VehicleSeasonAnalyseConfig),
            new VehicleMarkAnalyzerService(_applicationConfiguration.VehicleMarkAnalyseConfig),
            new VehicleTrafficAnalyzerService(_applicationConfiguration.VehicleTrafficAnalyseConfig),
            new VehicleDangerAnalyzerService(_applicationConfiguration.VehicleDangerAnalyseConfig));
    }
    

    #endregion

    
}