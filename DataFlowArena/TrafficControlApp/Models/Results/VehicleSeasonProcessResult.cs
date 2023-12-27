namespace TrafficControlApp.Models.Results;

public class VehicleSeasonProcessResult: IProcessResult
{
    public bool IsSucceed { get; set; }
    
    public string Message { get; set; }
    
    public int Data { get; set; }
}