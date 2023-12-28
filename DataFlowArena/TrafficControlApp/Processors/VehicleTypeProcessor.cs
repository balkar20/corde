using System.Threading.Tasks.Dataflow;
using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Storage;

namespace TrafficControlApp.Processors;

public class VehicleTypeProcessor(
    ISharedMemoryVehicleService sharedMemoryService,
    IVehicleAnalyzerService<IAnalysingResult> vehicleAnalyzerService,
    IMapper mapper)
    : Processor<Track>(sharedMemoryService, vehicleAnalyzerService, mapper)
{
    protected override async Task<IProcessResult> ProcessLogic(Track inputData)
    {
        var vehicles = await _sharedMemoryService.GetVehicleDataByTrackId(inputData.TrackId);
        
        foreach (var vehicle in vehicles)
        {
            var vehicleType = vehicle.VehicleType;
            
        }
        
        int bytesProcessed = 0;
        
        var testVeh = new Vehicle();
        var typeAnaliseResult = await vehicleAnalyzerService.Analyse(testVeh);
        var result = _mapper.Map<VehicleTypeProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleTypeProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }
}