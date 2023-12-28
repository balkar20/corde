using System.Threading.Tasks.Dataflow;
using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Processors;

public class VehicleMarkProcessor(
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
        await WorkWithDependentData(inputData.TrackId);
        var typeAnaliseResult = await _vehicleAnalyzerService.Analyse(testVeh);
        var result =  _mapper.Map<VehicleMarkProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleMarkProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
         _sharedMemoryService.VehicleColorStatisticsProcessResultDictionary.TryGetValue(trackId, out VehicleColorProcessResult dependentData);
         Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}