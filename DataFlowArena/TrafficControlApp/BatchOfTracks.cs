using TrafficControlApp.Models;

namespace TrafficControlApp;

public class BatchOfTracks
{
    public Queue<Track> Tracks { get; set; }
    
    public TimeFrame TimeFrame { get; set; }
}