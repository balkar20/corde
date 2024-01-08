using BenchmarkDotNet.Attributes;
using TrafficControlApp.ClientDevices.Devices;
using TrafficControlApp.Models;
using TrafficControlApp.Root;

namespace ProcessionBenchMarks;

public class TrackProcessionBenchmark
{
    [Params]
    private List<Track>? _tracks;

    [GlobalSetup]
    public void Setup()
    {
        var trackDevice = new TrackDevice();
        var bunch = trackDevice.GiveMeTrackDataBunch("").Result;
        _tracks = bunch.Tracks.ToList();
        
    }
    
    public void StartConsequesntly(){
        var startUpConfigurator = new TrafficControlStartupConfigurator();
        startUpConfigurator.Configure();
        
         startUpConfigurator.Test().Start();
    }
    
    public void StartParallel(){
        var startUpConfigurator = new TrafficControlStartupConfigurator();
        startUpConfigurator.Configure();
        
        startUpConfigurator.Run().Start();
    }
}