using System.Threading.Tasks.Dataflow;
using AutoMapper;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors.Abstractions;
using DataFlowProducerConsumer.Services;
using DataFlowProducerConsumer.Services.Storage;

namespace DataFlowProducerConsumer.Processors;

public class VehicleSeasonProcessor: Processor<Track>
{
    private readonly ISharedMemoryVehicleService _sharedMemoryService;
    private readonly IVehicleAnalyzerService<VehicleTrafficProcessResult> _analyserService;
    private readonly IMapper _mapper;

    public VehicleSeasonProcessor(ISharedMemoryVehicleService sharedMemoryService)
    {
        _sharedMemoryService = sharedMemoryService;
    }

    public override async Task<IProcessResult> ProcessLogic(Track inputData)
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