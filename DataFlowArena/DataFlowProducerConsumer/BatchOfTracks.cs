using DataFlowProducerConsumer.Models;

namespace DataFlowProducerConsumer;

public class BatchOfTracks
{
    public Queue<Track> Tracks { get; set; }
    
    public TimeFrame TimeFrame { get; set; }
}