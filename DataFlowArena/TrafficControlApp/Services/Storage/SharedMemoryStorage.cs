using AutoMapper.Internal;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Services.Storage;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Services;

class SharedMemoryStorage : ISharedMemoryStorage
{
    // private ISharedMemoryVehicleService _sharedMemoryVehicleServiceImplementation;
    private Dictionary<string, Vehicle> VehiclesByNumber;
    private Dictionary<string, List<Vehicle>> VehiclesByTrackId;
    
    // private Dictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary;
    // private Dictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary;
    // private Dictionary<string, VehicleColorStatisticsProcessResult> VehicleColorStatisticsProcessResultDictionary;
    
    

    public SharedMemoryStorage()
    {
        VehiclesByNumber = new();
        VehiclesByTrackId = new();
        
        VehicleColorStatisticsProcessResultDictionary = new();
        VehicleMarkProcessResultDictionary = new();
        VehicleTypeProcessResultDictionary = new();
    }
    
    public Dictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary { get;  }
    
    public Dictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary { get; }
    
    public Dictionary<string, VehicleColorProcessResult> VehicleColorStatisticsProcessResultDictionary { get; }
    
    public Dictionary<string, VehicleDangerProcessResult> VehicleDangerProcessResultDictionary { get; }
    
    public Dictionary<string, VehicleTrafficProcessResult> VehicleTrafficProcessResultDictionary { get; }
    
    
    public Dictionary<string, VehicleSeasonProcessResult> VehicleSeasonProcessResultDictionary { get; }
    
    
    // public Dictionary<string, VehicleTrafficProcessResult> VehicleProcessResultDictionary { get; }

    // public Vehicle GetVehicleDataByTrackId(string trackId)
    // {
    //     return _sharedMemoryVehicleServiceImplementation.GetVehicleDataByTrackId(trackId);
    // }

    public async Task<bool> AddVehicle(Vehicle vehicle)
    {
        return await Task.FromResult(VehiclesByNumber.TryAdd(vehicle.VehicleNumber, vehicle));

    }

    public async Task<bool> AddVehicleToTrack(Vehicle vehicle, string trackId)
    {
        if (VehiclesByTrackId.ContainsKey(trackId))
        {
            return VehiclesByTrackId[trackId].TryAdd(vehicle);
        }
        
        return await Task.FromResult(VehiclesByTrackId.TryAdd(trackId, new List<Vehicle>()
        {
            vehicle
        }));
    }

    Task<List<Vehicle>> ISharedMemoryStorage.GetVehicleDataByTrackId(string trackId)
    {
        if (VehiclesByTrackId.TryGetValue(trackId, out  var val))
        {
            return Task.FromResult(val);
        }

        return Task.FromResult(Enumerable.Empty<Vehicle>().ToList());

    }
    

    Task<Vehicle> ISharedMemoryStorage.GetVehicleDataByNumber(string number)
    {
        return  Task.FromResult(VehiclesByNumber[number]);
    }
}