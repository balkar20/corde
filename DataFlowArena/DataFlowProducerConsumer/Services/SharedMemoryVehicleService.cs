using AutoMapper.Internal;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Services;

class SharedMemoryVehicleService : ISharedMemoryVehicleService
{
    // private ISharedMemoryVehicleService _sharedMemoryVehicleServiceImplementation;
    private Dictionary<string, Vehicle> VehiclesByNumber;
    private Dictionary<string, List<Vehicle>> VehiclesByTrackId;
    
    // private Dictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary;
    // private Dictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary;
    // private Dictionary<string, VehicleColorStatisticsProcessResult> VehicleColorStatisticsProcessResultDictionary;
    
    

    public SharedMemoryVehicleService()
    {
        VehiclesByNumber = new();
        VehiclesByTrackId = new();
        
        VehicleColorStatisticsProcessResultDictionary = new();
        VehicleMarkProcessResultDictionary = new();
        VehicleTypeProcessResultDictionary = new();
    }
    
    public Dictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary { get;  }
    public Dictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary { get; }
    public Dictionary<string, VehicleColorStatisticsProcessResult> VehicleColorStatisticsProcessResultDictionary { get; }

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

    Task<List<Vehicle>> ISharedMemoryVehicleService.GetVehicleDataByTrackId(string trackId)
    {
        return Task.FromResult(VehiclesByTrackId[trackId]);
    }
    

    Task<Vehicle> ISharedMemoryVehicleService.GetVehicleDataByNumber(string number)
    {
        return  Task.FromResult(VehiclesByNumber[number]);
    }
}