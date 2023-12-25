namespace DataFlowProducerConsumer.Models.Results;

public class VehicleColorStatisticsProcessResult: IProcessResult
{
    public bool IsSucceed { get; set; }
    
    public string Message { get; set; }
    
    public int Data { get; set; }
}