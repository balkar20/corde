using TrafficControlApp.Models;
using TrafficControlApp.Processors;

namespace TrafficControlApp.Services;

public interface IVehicleAnalyzerService<TAnalyseResult>
{
    Task<TAnalyseResult> Analyse(Vehicle vehicle);
}