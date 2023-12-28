using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Processors;

public class VehicleColorProcessor(
        ISharedMemoryStorage sharedMemoryService,
        IVehicleAnalyzerService<IAnalysingResult> vehicleAnalyzerService,
        IMapper mapper,
        IEventLoggingService eventLoggingService)
    : Processor<Track>(sharedMemoryService, vehicleAnalyzerService, mapper, eventLoggingService)
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
        var typeAnaliseResult = await _vehicleAnalyzerService.Analyse(testVeh);
        var result = _mapper.Map<VehicleColorProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleColorStatisticsProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
        _sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessResult dependentData);
        Console.WriteLine($"DependentData(VehicleColorProcessResult ) Message: {dependentData.Message}");
    }
}