namespace DataFlowProducerConsumer.Models.Results;

public class VehicleTypeProcessResult: IProcessResult
{
    public bool IsSuccessed { get; set; }
    
    public string Message { get; set; }
    
    public int Data { get; set; }
    
    
}