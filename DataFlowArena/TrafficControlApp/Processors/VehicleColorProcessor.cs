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

public class VehicleColorProcessor: Processor<Track>
{
    public VehicleColorProcessor(
        ISharedMemoryVehicleService sharedMemoryService,
        IVehicleAnalyzerService<IAnalysingResult> vehicleAnalyzerService, 
        IMapper mapper) : 
        base(sharedMemoryService, vehicleAnalyzerService, mapper)
    {
    }

    protected override async Task<IProcessResult> ProcessLogic(Track inputData)
    {
        var vehicles = await _sharedMemoryService.GetVehicleDataByTrackId(inputData.TrackId);
        
        foreach (var vehicle in vehicles)
        {
            var vehicleType = vehicle.VehicleType;
            
        }
        
        int bytesProcessed = 0;
        
        var testVeh = new Vehicle();
        var typeAnaliseResult = await _vehicleAnalyzerService.Analyse(testVeh);
        var result = _mapper.Map<VehicleColorProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleColorStatisticsProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
        _sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessResult dependentData);
        Console.WriteLine($"DependentDta(VehicleMarkProcessResult ) Message: {dependentData.Message}");
    }
}