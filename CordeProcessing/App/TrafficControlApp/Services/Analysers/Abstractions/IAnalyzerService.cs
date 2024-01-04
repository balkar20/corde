using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results.Analyse.Abstractions;

namespace TrafficControlApp.Services.Analysers.Abstractions;

public interface IAnalyzerService
{
    Task<IAnalysingResult> Analyse(AnalysingItem analysingItem);
}