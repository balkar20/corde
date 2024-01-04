using TrafficControlApp.Models;

namespace TrafficControlApp.ClientDevices.Abstractions;

public interface ITrackDevice
{
    public Task<BatchOfTracks> GiveMeTrackDataBunch(string batchType);
}