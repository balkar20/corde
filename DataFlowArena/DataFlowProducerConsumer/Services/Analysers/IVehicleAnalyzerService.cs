using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Processors;

namespace DataFlowProducerConsumer.Services;

internal interface IVehicleAnalyzerService<TAnalyseResult>
{
    Task<TAnalyseResult> Analyse(Vehicle vehicle);
}