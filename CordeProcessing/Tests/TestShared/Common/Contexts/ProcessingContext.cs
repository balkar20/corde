using Moq;
using ParallelProcessing.Models;
using ParallelProcessing.Processors.Abstractions;

namespace Common.Contexts;

public class ProcessingContext
{
    public Mock<IProgressiveProcessor<Track>> VehicleTypeProcessor { get; }
    public Mock<IProgressiveProcessor<Track>> ColorProcessor { get; }
    public Mock<IProgressiveProcessor<Track>> SeasonProcessor { get; }
    public Mock<IProgressiveProcessor<Track>> MarkProcessor { get; }
    public Mock<IProgressiveProcessor<Track>> TrafficProcessor { get; }
    public Mock<IProgressiveProcessor<Track>> DangerProcessor { get; }
}