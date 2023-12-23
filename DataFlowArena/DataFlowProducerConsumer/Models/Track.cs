using System.Drawing;

namespace DataFlowProducerConsumer.Models;

public class Track
{
    public string TrackId { get; set; }
    public List<Point> Points { get; set; }
    public double AverageSpeed { get; set; }
    // public Queue<Frame> Frames { get; set; }
}