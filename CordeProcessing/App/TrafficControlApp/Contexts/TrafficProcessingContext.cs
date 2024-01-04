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

public class TrafficProcessingContext(ISharedMemoryStorage sharedMemoryStorage, ApplicationConfiguration applicationConfiguration)
{
    private readonly ISharedMemoryStorage _sharedMemoryStorage = sharedMemoryStorage;
    private readonly IEventLoggingService _logger;

    #region Processors

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
        var aso = new TypeAnalyzerService(applicationConfiguration.VehicleTypeAnalyseConfig);
        var asi = new ColorAnalyzerService(applicationConfiguration.VehicleColorAnalyseConfig);
        return (
            new TypeAnalyzerService(applicationConfiguration.VehicleTypeAnalyseConfig),
            new ColorAnalyzerService(applicationConfiguration.VehicleColorAnalyseConfig),
            new SeasonAnalyzerService(applicationConfiguration.VehicleSeasonAnalyseConfig),
            new MarkAnalyzerService(applicationConfiguration.VehicleMarkAnalyseConfig),
            new TrafficAnalyzerService(applicationConfiguration.VehicleTrafficAnalyseConfig),
            new DangerAnalyzerService(applicationConfiguration.VehicleDangerAnalyseConfig));
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
        var aso = new TypeAnalyzerService(applicationConfiguration.VehicleTypeAnalyseConfig);
        var _sharedMemoryStorage = new SharedMemoryStorage();
        return (
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage),
            new ColorAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage),
            new SeasonAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage),
            new MarkAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage),
            new TrafficAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage),
            new DangerAbstractDictionaryProcessingItemsStorageServiceRepository(_sharedMemoryStorage));
    }
    
    #endregion

    
}