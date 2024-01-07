using Moq;
using TrafficControlApp.Services.Analysers.Abstractions;

namespace Common.Contexts;

public class AnalysingContext
{
    public Mock<IAnalyzerService> VehicleTypeAnalyzerService { get; }
    public Mock<IAnalyzerService> ColorAnalyzerService { get; }
    public Mock<IAnalyzerService> SeasonAnalyzerService { get; }
    public Mock<IAnalyzerService> MarkAnalyzerService { get; }
    public Mock<IAnalyzerService> TrafficAnalyzerService { get; }
    public Mock<IAnalyzerService> DangerAnalyzerService { get; }
}