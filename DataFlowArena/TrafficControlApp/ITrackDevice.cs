using Microsoft.VisualBasic;

namespace TrafficControlApp;

public interface ITrackDevice
{
    public Task<BatchOfTracks> GiveMeTrackDataBunch(string batchType);
}