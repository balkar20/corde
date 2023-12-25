using System.Threading.Tasks.Dataflow;
using AutoMapper;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors.Abstractions;
using DataFlowProducerConsumer.Services;

namespace DataFlowProducerConsumer.Processors;

public class VehicleMarkProcessor: Processor<Track, VehicleMarkProcessResult>
{
    private readonly ISharedMemoryVehicleService _sharedMemoryService;
    private readonly IVehicleAnalyzerService<VehicleMarkProcessResult> _analyserService;
    private readonly IMapper _mapper;

    public VehicleMarkProcessor(ISharedMemoryVehicleService sharedMemoryService)
    {
        _sharedMemoryService = sharedMemoryService;
    }

    public string TrackId { get; set; }
    public bool CanRun { get; set; }
    public bool Completed { get; set; }

    public override async Task<VehicleMarkProcessResult> ProcessLogic(Track inputData)
    {
        var vehicles = await _sharedMemoryService.GetVehicleDataByTrackId(inputData.TrackId);
        
        foreach (var vehicle in vehicles)
        {
            var vehicleType = vehicle.VehicleType;
            
        }
        
        int bytesProcessed = 0;
        
        var testVeh = new Vehicle();
        var typeAnaliseResult = await _analyserService.Analyse(testVeh);
        return _mapper.Map<VehicleMarkProcessResult>(typeAnaliseResult);
    }

    // public async ValueTask<VehicleTypeProcessResult> ProcessAsync(Track inputData)
    // {
    //     var vehicles = await _sharedMemoryService.GetVehicleDataByTrackId(inputData.TrackId);
    //     
    //     foreach (var vehicle in vehicles)
    //     {
    //         var vehicleType = vehicle.VehicleType;
    //         
    //     }
    //    
    //     int bytesProcessed = 0;
    //
    //     var testVeh = new Vehicle();
    //     var typeAnaliseResult = await _analyserService.Analyse(testVeh);
    //     VehicleTypeProcessResult result = _mapper.Map<VehicleTypeProcessResult>(typeAnaliseResult);
    //     
    //     return result;
    // }
}