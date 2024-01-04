using System.Collections.Concurrent;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;

namespace TrafficControlApp.Services.Storage.Abstractions;

public interface ISharedMemoryStorage
{
    ConcurrentDictionary<string, Track> ProcessingItemsStorage{ get; set; }
    ConcurrentDictionary<string, VehicleTypeProcessionResult> ProcessionTypeResultStorage{ get; set; }
    ConcurrentDictionary<string, VehicleMarkProcessionResult> ProcessionMarkResultStorage{ get; set; }
    ConcurrentDictionary<string, VehicleColorProcessionResult> ProcessionColorResultStorage{ get; set; }
    ConcurrentDictionary<string, VehicleSeasonProcessionResult> ProcessionSeasonResultStorage{ get; set; }
    ConcurrentDictionary<string, VehicleDangerProcessionResult> ProcessionDangerResultStorage{ get; set; }
    ConcurrentDictionary<string, VehicleTrafficProcessionResult> ProcessionTrafficResultStorage{ get; set; }
}