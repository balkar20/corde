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

public class VehicleMarkProcessor: Processor<Track>
{
    private readonly ISharedMemoryVehicleService _sharedMemoryService;
    private readonly IVehicleAnalyzerService<IAnalysingResult> _analyserService;
    private readonly IMapper _mapper;

    public VehicleMarkProcessor(ISharedMemoryVehicleService sharedMemoryService,
        IVehicleAnalyzerService<IAnalysingResult> vehicleAnalyzerService, 
        IMapper mapper) : 
        base(sharedMemoryService, vehicleAnalyzerService, mapper)
    {
        _sharedMemoryService = sharedMemoryService;
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
        var typeAnaliseResult = await _analyserService.Analyse(testVeh);
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