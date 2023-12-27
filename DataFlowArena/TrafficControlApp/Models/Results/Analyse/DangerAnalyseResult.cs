using TrafficControlApp.Models.Results.Analyse.Abstractions;

namespace TrafficControlApp.Models.Results.Analyse;

public class DangerAnalyseResult: IAnalysingResult
{
    public string Message { get; set; }
}