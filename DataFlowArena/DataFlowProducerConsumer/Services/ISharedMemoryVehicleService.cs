using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;

namespace DataFlowProducerConsumer.Services;

public interface ISharedMemoryVehicleService
{
    Dictionary<string, VehicleTypeProcessResult> VehicleTypeProcessResultDictionary { get;  }
    Dictionary<string, VehicleMarkProcessResult> VehicleMarkProcessResultDictionary{ get; }
    Dictionary<string, VehicleColorStatisticsProcessResult> VehicleColorStatisticsProcessResultDictionary{ get; }
    Task<List<Vehicle>> GetVehicleDataByTrackId(string trackId);
    Task<Vehicle> GetVehicleDataByNumber(string trackId);
    Task<bool>  AddVehicle(Vehicle vehicle);
    Task<bool>  AddVehicleToTrack(Vehicle vehicle, string trackId);
}