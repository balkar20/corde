using TrafficControlApp.Processors.Abstractions;

namespace TrafficControlApp.Models.ProcessingLog;

public class ProcessionInfo
{
    public string ProcessorName { get; set; }
    public string ProcessorType { get; set; }
    public string NextDependentProcessorName { get; set; }
}