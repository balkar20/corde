using System.Collections.Concurrent;
using TrafficControlApp.Exceptions;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Procession.Abstractions;

namespace TrafficControlApp.Services.Storage.Services;

public  class TypeAbstractDictionaryProcessingItemsStorageServiceRepository: IProcessingItemsStorageServiceRepository<string, Track>
{
    private ConcurrentDictionary<string, Track> ProcessingItemsStorage => new ();
    private ConcurrentDictionary<string, VehicleTypeProcessionResult> ProcessionTypeResultStorage => new ();

    public async Task CreateProcessingItem(Track processItem)
    {
        if (!ProcessingItemsStorage.TryAdd(Guid.NewGuid().ToString(), processItem))
        {
            throw new ProcessingItemCreationException(processItem);
        }
    }


    public async Task CreateProcessingItemResult(VehicleTypeProcessionResult result)
    {
        if (!ProcessionTypeResultStorage.TryAdd(Guid.NewGuid().ToString(), result))
        {
            throw new ProcessingItemResultCreationException(result);
        }
    }

    public virtual async Task<Track> GetProcessingItem(string processItemKey)
    {
        ProcessingItemsStorage.TryGetValue(processItemKey, out var result);
        return result;
    }

    public virtual async Task<IProcessionResult> GetProcessingItemResult(string processItemKey)
    {
        ProcessionTypeResultStorage.TryGetValue(processItemKey, out var result);
        return result;
    }
}

class TypeAbstractDictionaryProcessingItemsStorageServiceRepositoryImpl : TypeAbstractDictionaryProcessingItemsStorageServiceRepository
{
    
}