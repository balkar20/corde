namespace DataFlowProducerConsumer.Models.Results;

public class VehicleDangerProcessResult: IProcessResult
{
    public bool IsSucceed { get; set; }
    
    public string Message { get; set; }
    
    public int Data { get; set; }
}