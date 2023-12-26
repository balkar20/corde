using System.Drawing;
using DataFlowProducerConsumer.Models;

namespace DataFlowProducerConsumer;

class TrackDevice : ITrackDevice
{
    public Task<BatchOfTracks> GiveMeTrackDataBunch(string batchType)
    {
        return Task.FromResult<BatchOfTracks>(GetRandomData(20));
    }

    private BatchOfTracks GetRandomData(int amount)
    {
        return new BatchOfTracks
        {
            Tracks = GetRandomTrackData(20),
            TimeFrame = new TimeFrame(DateTime.Now, DateTime.Now + TimeSpan.FromSeconds(50))
        };
    }

    private Queue<FramePoint> GetRandomPointData(int amount)
    {
        var rand = new Random();
        var rand2 = new Random();
        
        Queue<FramePoint> fttrackQueue = new Queue<FramePoint>()
        {
                
        };
        
        
        for (int i = 0; i < amount; i++)
        {
            fttrackQueue.Enqueue(new FramePoint()
            {
                Point = new Point()
                {
                    X = rand.Next(),
                    Y = rand.Next()
                }
            });
        }
        return fttrackQueue;
    }

    private Queue<Track> GetRandomTrackData(int amount)
    {
        var rand = new Random();
        var rand2 = new Random();
        
        Queue<Track> fttrackQueue = new Queue<Track>()
        {
                
        };
        
        
        for (int i = 0; i < amount; i++)
        {
            fttrackQueue.Enqueue(new Track()
            {
                TrackId = rand.Next().ToString(),
                AverageSpeed = rand.Next(50,90),
                Points = GetRandomPointData(13)
            });
        }
        return fttrackQueue;
    }
}