using Moq;
using TrafficControlApp.ClientDevices.Devices;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Storage.Services;

namespace TrafficControlApp.Tests.StartUp;

public class RunTest
{
    private readonly Mock<IAnalyzerService> vehicleTypeAnalyzerService;
    private readonly Mock<IAnalyzerService> colorAnalyzerService;
    private readonly Mock<IAnalyzerService> seasonAnalyzerService;
    private readonly Mock<IAnalyzerService> markAnalyzerService;
    private readonly Mock<IAnalyzerService> trafficAnalyzerService;
    private readonly Mock<IAnalyzerService> dangerAnalyzerService;

    public RunTest()
    {
        vehicleTypeAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        colorAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        seasonAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        markAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        trafficAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        dangerAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
    }

    [Fact]
    public async Task Run_First_Dependency_Type_Passed_In_Right_Order()
    {
        var _trackDevice = new TrackDevice();
       var bunchOfTracks = await _trackDevice.GiveMeTrackDataBunch("");
        TypeAbstractDictionaryProcessingItemsStorageServiceRepository abst =
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository();

        var track = bunchOfTracks.Tracks.Dequeue();
        abst.CreateProcessingItem(track);
        abst.CreateProcessingItemResult(new VehicleTypeProcessionResult()
        {
            ItemId = track.ItemId
        });

        var foo = abst.GetProcessingItemResult(track.ItemId);
    }
}