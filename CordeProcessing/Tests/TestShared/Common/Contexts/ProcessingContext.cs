using Moq;
using ParallelProcessing.Models;
using ParallelProcessing.Processors.Abstractions;

namespace Common.Contexts;

public class ProcessingContext
{
    public Mock<IProcessor<Track>> VehicleTypeProcessor { get; }
    public Mock<IProcessor<Track>> ColorProcessor { get; }
    public Mock<IProcessor<Track>> SeasonProcessor { get; }
    public Mock<IProcessor<Track>> MarkProcessor { get; }
    public Mock<IProcessor<Track>> TrafficProcessor { get; }
    public Mock<IProcessor<Track>> DangerProcessor { get; }
}