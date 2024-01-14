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
        int amountOfProcessors = 9;
        var bunch = trackDevice.GiveMeTrackDataBunch("", amountOfProcessors).Result;
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