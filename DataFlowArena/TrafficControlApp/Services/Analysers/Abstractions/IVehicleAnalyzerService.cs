using TrafficControlApp.Models;

namespace TrafficControlApp.Services.Analysers.Abstractions;

public interface IVehicleAnalyzerService<TAnalyseResult>
{
    Task<TAnalyseResult> Analyse(Vehicle vehicle);
}