using TrafficControlApp.Models.Items.Processing;

namespace TrafficControlApp.Models;

public class Track: ProcessingItem
{
    public Queue<FramePoint> Points { get; set; }
    public double AverageSpeed { get; set; }
    public TimeFrame TimeFrame { get; set; }
    public string VehicleNumber { get; set; }
}