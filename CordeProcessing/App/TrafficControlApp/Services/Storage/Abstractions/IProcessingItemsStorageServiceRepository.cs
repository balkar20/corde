﻿using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results.Procession.Abstractions;

namespace TrafficControlApp.Services;

public interface IProcessingItemsStorageServiceRepository<TProcessingItemKey, TProcessingItem>
{
    Task CreateProcessingItem(TProcessingItem processItem);
    Task<TProcessingItem> GetProcessingItem(TProcessingItemKey processItemKey);
    Task<IProcessionResult> GetProcessingItemResult(TProcessingItemKey processItemKey);

}