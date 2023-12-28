using AutoMapper;
using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers;
using TrafficControlApp.Services.Storage;

namespace TrafficControlApp.Contexts;

public class TrafficProcessingContext
{
    private readonly ISharedMemoryVehicleService _sharedMemoryVehicleService;
    private readonly ApplicationConfiguration _applicationConfiguration;

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
        VehicleRootProcessor = new VehicleTypeProcessor(_sharedMemoryVehicleService, analysers.vehicleTypeAnalyzerService, mapper);
        VehicleSeasonProcessor = new VehicleSeasonProcessor(_sharedMemoryVehicleService, analysers.seasonAnalyzerService, mapper);
        VehicleColorProcessor = new VehicleColorProcessor(_sharedMemoryVehicleService, analysers.colorAnalyzerService, mapper);
        VehicleMarkProcessor = new VehicleMarkProcessor(_sharedMemoryVehicleService, analysers.markAnalyzerService, mapper);
        VehicleTrafficProcessor = new VehicleTrafficProcessor(_sharedMemoryVehicleService, analysers.trafficAnalyzerService, mapper);
        VehicleDangerProcessor = new VehicleDangerProcessor(_sharedMemoryVehicleService, analysers.dangerAnalyzerService, mapper);
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