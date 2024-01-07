using System.Collections.Concurrent;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Services;

/// <summary>
/// Shared Memory Operation should be Thread safe
/// </summary>
public class SharedMemoryStorage : ISharedMemoryStorage
{
    public ConcurrentDictionary<string, Track> ProcessingItemsStorage { get; set; } = new();
    
    public ConcurrentDictionary<string, VehicleTypeProcessionResult> ProcessionTypeResultStorage { get; set; } = new();
    
    public ConcurrentDictionary<string, VehicleMarkProcessionResult> ProcessionMarkResultStorage { get; set; } = new();

    public ConcurrentDictionary<string, VehicleColorProcessionResult> ProcessionColorResultStorage { get; set; } = new();

    public ConcurrentDictionary<string, VehicleSeasonProcessionResult> ProcessionSeasonResultStorage { get; set; } = new();

    public ConcurrentDictionary<string, VehicleDangerProcessionResult> ProcessionDangerResultStorage { get; set; } = new();

    public ConcurrentDictionary<string, VehicleTrafficProcessionResult> ProcessionTrafficResultStorage { get; set; } = new();
}