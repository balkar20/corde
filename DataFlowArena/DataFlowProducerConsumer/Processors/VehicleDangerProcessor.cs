using System.Threading.Tasks.Dataflow;
using AutoMapper;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors.Abstractions;
using DataFlowProducerConsumer.Services;
using DataFlowProducerConsumer.Services.Storage;

namespace DataFlowProducerConsumer.Processors;

public class VehicleDangerProcessor: Processor<Track>
{
    private readonly ISharedMemoryVehicleService _sharedMemoryService;
    private readonly IVehicleAnalyzerService<VehicleDangerProcessResult> _analyserService;
    private readonly IMapper _mapper;

    public VehicleDangerProcessor(ISharedMemoryVehicleService sharedMemoryService)
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
        var result =  _mapper.Map<VehicleDangerProcessResult>(typeAnaliseResult);
        _sharedMemoryService.VehicleDangerProcessResultDictionary.Add(inputData.TrackId, result);
        return result;
    }

    private async Task WorkWithDependentData(string trackId)
    {
         _sharedMemoryService.VehicleTypeProcessResultDictionary.TryGetValue(trackId, out VehicleTypeProcessResult dependentData);
         Console.WriteLine($"DependentDta(VehicleColorStatistics) Message: {dependentData.Message}");
    }
}