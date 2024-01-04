using TrafficControlApp.Models.Results.Procession.Abstractions;

namespace TrafficControlApp.Models.Results;

public class PoolProcessionResult:IProcessionResult
{
    public bool IsSucceed { get; set; }
    
    public string ItemId { get; set; }
}