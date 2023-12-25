using Microsoft.VisualBasic;

namespace DataFlowProducerConsumer;

public interface ITrackDevice
{
    public Task<BatchOfTracks> GiveMeTrackDataBunch(string batchType);
}