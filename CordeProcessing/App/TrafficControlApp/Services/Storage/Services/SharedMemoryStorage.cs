using System.Collections.Concurrent;
using AutoMapper.Internal;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Services;

/// <summary>
/// Shared Memory Operation should be Thread safe
/// </summary>
class SharedMemoryStorage : ISharedMemoryStorage
{
    // private ISharedMemoryVehicleService _sharedMemoryVehicleServiceImplementation;
    
    // private ConcurrentDictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary;
    // private ConcurrentDictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary;
    // private ConcurrentDictionary<string, VehicleColorStatisticsProcessResult> VehicleColorStatisticsProcessResultDictionary;


    // public IProcessingItemsStorageServiceRepository<VehicleTypeProcessionResult> ProcessingItemsStorageServiceRepository { get; }
    // public IAnalysingItemsStorageRepository<AnalysingItem, string> AnalysingItemsStorageRepository { get; }
    // public ConcurrentDictionary<string, VehicleTypeProcessionResult> VehicleTypeProcessResultDictionary { get;  } = new();
    //
    // public ConcurrentDictionary<string, VehicleMarkProcessionResult> VehicleMarkProcessResultDictionary { get; } = new();
    //
    // public ConcurrentDictionary<string, VehicleColorProcessionResult> VehicleColorStatisticsProcessResultDictionary { get; } = new();
    //
    // public ConcurrentDictionary<string, VehicleDangerProcessionResult> VehicleDangerProcessResultDictionary { get; }
    //
    // public ConcurrentDictionary<string, VehicleTrafficProcessionResult> VehicleTrafficProcessResultDictionary { get; }
    //
    //
    // public ConcurrentDictionary<string, VehicleSeasonProcessionResult> VehicleSeasonProcessResultDictionary { get; }
    
    
    // public ConcurrentDictionary<string, VehicleTrafficProcessResult> VehicleProcessResultDictionary { get; }

    // public Vehicle GetVehicleDataByTrackId(string trackId)
    // {
    //     return _sharedMemoryVehicleServiceImplementation.GetVehicleDataByTrackId(trackId);
    // }
    
}