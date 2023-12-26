using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Producers.Abstraction;

namespace DataFlowProducerConsumer.Producers;

public class TrackProducer: ITrackProducer
{
    private readonly ITrackDevice _trackDevice;
    private readonly ApplicationConfiguration config;

    public TrackProducer(ITrackDevice trackDevice, ApplicationConfiguration config)
    {
        _trackDevice = trackDevice;
        this.config = config;
    }

    public SortedSet<int> Treads { get; set; }
    public async Task ProduceAllAsync(ITargetBlock<Track> target)
    {
        var sourceData = await _trackDevice.GiveMeTrackDataBunch("SomeTracks");
        foreach (var track in sourceData.Tracks)
        {
            var item = $"Item {track.TrackId + 1}";
            // Console.WriteLine($"Producing {item}, ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
            await ProduceOne(target, track);
            
            
            Treads.Add(Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine($"Produced {item}? ThreadId is: {Thread.CurrentThread.ManagedThreadId}, ThreadPool ?? {Thread.CurrentThread.IsThreadPoolThread}");
            Console.WriteLine($"Current Ammount Of threads = {Treads.Count}");
            //
            // await target.SendAsync(track);
        }

        target.Complete();
    }

    private async Task ProduceOne(ITargetBlock<Track> target, Track track)
    {
        //emulating wait for period
        await Task.Delay(config.ProduceSpeed);
        await target.SendAsync(track);
    }
}