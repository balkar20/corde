using TrafficControlApp.Exceptions;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Services.Storage.Services;

public  class TrafficAbstractDictionaryProcessingItemsStorageServiceRepository: IProcessingItemsStorageServiceRepository<string, Track>
{
    private ISharedMemoryStorage sharedMemoryStorage;

    public TrafficAbstractDictionaryProcessingItemsStorageServiceRepository(ISharedMemoryStorage sharedMemoryStorage)
    {
        this.sharedMemoryStorage = sharedMemoryStorage;
    }

    public async Task CreateProcessingItem(Track processItem)
    {
        if (!sharedMemoryStorage.ProcessingItemsStorage.TryAdd(Guid.NewGuid().ToString(), processItem))
        {
            throw new ProcessingItemCreationException(processItem);
        }
    }


    public async Task CreateProcessingItemResult(VehicleTrafficProcessionResult result)
    {
        if (!sharedMemoryStorage.ProcessionTrafficResultStorage.TryAdd(Guid.NewGuid().ToString(), result))
        {
            throw new ProcessingItemResultCreationException(result);
        }
    }

    public virtual async Task<Track> GetProcessingItem(string processItemKey)
    {
        sharedMemoryStorage.ProcessingItemsStorage.TryGetValue(processItemKey, out var result);
        return result;
    }

    public virtual async Task<IProcessionResult> GetProcessingItemResult(string processItemKey)
    {
        sharedMemoryStorage.ProcessionTrafficResultStorage.TryGetValue(processItemKey, out var result);
        return result;
    }
}