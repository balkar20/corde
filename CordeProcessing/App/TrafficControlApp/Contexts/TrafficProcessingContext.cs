using AutoMapper;
using TrafficControlApp.Config;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Analysers.Services;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage.Abstractions;
using TrafficControlApp.Services.Storage.Services;

namespace TrafficControlApp.Contexts;

public class TrafficProcessingContext
{
    private readonly ISharedMemoryStorage _sharedMemoryStorage;
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IEventLoggingService _logger;

    public TrafficProcessingContext(ISharedMemoryStorage sharedMemoryStorage, ApplicationConfiguration applicationConfiguration)
    {
        _sharedMemoryStorage = sharedMemoryStorage;
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
        var repositories = GetRepositories();
        VehicleRootProcessor = new VehicleTypeProcessor(repositories.vehicleTypeRepository, analysers.vehicleTypeAnalyzerService, mapper, _logger);
        VehicleSeasonProcessor = new VehicleSeasonProcessor(repositories.seasonRepository, analysers.seasonAnalyzerService, mapper, _logger);
        VehicleColorProcessor = new VehicleColorProcessor(repositories.colorRepository, analysers.colorAnalyzerService, mapper, _logger);
        VehicleMarkProcessor = new VehicleMarkProcessor(repositories.markRepository, analysers.markAnalyzerService, mapper, _logger);
        VehicleTrafficProcessor = new VehicleTrafficProcessor(repositories.trafficRepository, analysers.trafficAnalyzerService, mapper, _logger);
        VehicleDangerProcessor = new VehicleDangerProcessor(repositories.dangerRepository, analysers.dangerAnalyzerService, mapper, _logger);
    }

    #endregion

    #region Private Methods

        
    private (
        IAnalyzerService vehicleTypeAnalyzerService,
        IAnalyzerService colorAnalyzerService,
        IAnalyzerService seasonAnalyzerService,
        IAnalyzerService markAnalyzerService,
        IAnalyzerService trafficAnalyzerService,
        IAnalyzerService dangerAnalyzerService
        ) GetAnalysers()
    {
        var aso = new TypeAnalyzerService(_applicationConfiguration.VehicleTypeAnalyseConfig);
        var asi = new ColorAnalyzerService(_applicationConfiguration.VehicleColorAnalyseConfig);
        return (
            new TypeAnalyzerService(_applicationConfiguration.VehicleTypeAnalyseConfig),
            new ColorAnalyzerService(_applicationConfiguration.VehicleColorAnalyseConfig),
            new SeasonAnalyzerService(_applicationConfiguration.VehicleSeasonAnalyseConfig),
            new MarkAnalyzerService(_applicationConfiguration.VehicleMarkAnalyseConfig),
            new TrafficAnalyzerService(_applicationConfiguration.VehicleTrafficAnalyseConfig),
            new DangerAnalyzerService(_applicationConfiguration.VehicleDangerAnalyseConfig));
    }

        
    private (
        IProcessingItemsStorageServiceRepository<String,  Track> vehicleTypeRepository,
        IProcessingItemsStorageServiceRepository<String,  Track> colorRepository,
        IProcessingItemsStorageServiceRepository<String,  Track> seasonRepository,
        IProcessingItemsStorageServiceRepository<String,  Track> markRepository,
        IProcessingItemsStorageServiceRepository<String,  Track> trafficRepository,
        IProcessingItemsStorageServiceRepository<String,  Track> dangerRepository
        ) GetRepositories()
    {
        var aso = new TypeAnalyzerService(_applicationConfiguration.VehicleTypeAnalyseConfig);
        var asi = new ColorAnalyzerService(_applicationConfiguration.VehicleColorAnalyseConfig);
        return (
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(),
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(),
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(),
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(),
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(),
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository());
    }


    #endregion

    
}