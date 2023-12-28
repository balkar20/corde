namespace TrafficControlApp.Models;

public class Track
{
    public string TrackId { get; set; }
    public Queue<FramePoint> Points { get; set; }
    public double AverageSpeed { get; set; }
    public TimeFrame TimeFrame { get; set; }
    public string VehicleNumber { get; set; }
}