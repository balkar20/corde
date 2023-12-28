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

public class VehicleSeasonProcessor: Processor<Track>
{
    public VehicleSeasonProcessor(ISharedMemoryVehicleService sharedMemoryService,
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
        await WorkWithDependentData(inputData.TrackId);
        var typeAnaliseResult = await _vehicleAnalyzerService.Analyse(testVeh);
        var result =  _mapper.Map<VehicleTrafficProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleTrafficProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
         _sharedMemoryService.VehicleMarkProcessResultDictionary.TryGetValue(trackId, out VehicleMarkProcessResult dependentData);
         Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}