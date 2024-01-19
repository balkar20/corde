using System.Collections;
using AutoMapper;
using BenchmarkDotNet.Jobs;
using Moq;
using ParallelProcessing.ClientDevices.Devices;
using ParallelProcessing.Mapping;
using ParallelProcessing.Models;
using ParallelProcessing.Models.Items.Analysing;
using ParallelProcessing.Models.Results;
using ParallelProcessing.Services;
using ParallelProcessing.Services.Analysers.Abstractions;
using ParallelProcessing.Services.Storage.Services;

namespace ParallelProcessing.Tests.StartUp;

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
        // vehicleTypeAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        // colorAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        // seasonAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        // markAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        // trafficAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
        // dangerAnalyzerService.Setup(p => p.Analyse(It.IsAny<AnalysingItem>()));
    }

    [Fact]
    public async Task Run_First_Dependency_Type_Passed_In_Right_Order()
    {
        var _trackDevice = new TrackDevice();
        var amountOfProcessors = 6;
       var bunchOfTracks = await _trackDevice.GiveMeTrackDataBunch("", amountOfProcessors);
        TypeAbstractDictionaryProcessingItemsStorageServiceRepository abst =
            new TypeAbstractDictionaryProcessingItemsStorageServiceRepository(new SharedMemoryStorage());

        var track = bunchOfTracks.Tracks.Dequeue();
        await abst.CreateProcessingItem(track);
        await abst.CreateProcessingItemResult(new VehicleTypeProcessionResult()
        {
            ItemId = track.ItemId
        });

        var foo = abst.GetProcessingItemResult(track.ItemId);
    }

    [Fact]
    public async Task MapperTest()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TrackProcessingMappingProfile());  //mapping between Web and Business layer objects// mapping between Business and DB layer objects
        });
        var _mapper = config.CreateMapper();
        var res = _mapper.Map<TypeAnalysingItem>(new Track());
    }

    [Fact]
    public async Task QueueTest()
    {
        var que = new Queue<string>();
        que.Enqueue("one");
        que.Enqueue("two");
        que.Enqueue("three");
        Queue<string> myQueue2 = new Queue<string>(que);
        var q2Deq = myQueue2.Dequeue(); 
        var q1Deq = que.Dequeue(); 
        
        Assert.True(q2Deq == "one");
        Assert.True(q1Deq == "one");
    }
    
    public static void PrintValues(IEnumerable myCollection) 
    { 
        // This method prints all the 
        // elements in the Stack. 
        foreach(Object obj in myCollection) 
            Console.WriteLine(obj); 
        Console.WriteLine(); 
    } 
}