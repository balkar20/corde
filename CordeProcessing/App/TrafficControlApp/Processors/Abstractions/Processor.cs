using AutoMapper;
using TrafficControlApp.Models.Items.Processing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse.Abstractions;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services;
using TrafficControlApp.Services.Analysers.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;
using TrafficControlApp.Services.Storage.Abstractions;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput>(
        IEventLoggingService? _eventLoggingService)
    : IProcessor<TInput>
{
    #region private fields

    // protected readonly IAnalyzerService _analyzerService = _analyzerService;
    // protected readonly IProcessingItemsStorageServiceRepository<string> _processingItemsStorageServiceRepository = _processingItemsStorageServiceRepository;
    // protected readonly IMapper _mapper = _mapper;
    protected readonly IEventLoggingService? _eventLoggingService = _eventLoggingService;

    #endregion

    #region Public Properties

    public bool CanRun { get; set; }

    public string ProcessorId { get; set; }

    public string ProcessId { get; set; }

    public string InputId { get; set; }

    public bool CompletedWithDependentProcessors { get; set; }

    public Queue<IProcessor<TInput>> ProcessorsDepended => new();

    public IProcessor<TInput> CurrentCallingProcessorInCaseQueueIsEmpty { get; set; }

    public bool NextInQueHasNoDependencies { get; set; }

    #endregion


    #region Protected Abstract Methods

    protected abstract Task<IProcessionResult> ProcessLogic(TInput inputData);

    #endregion

    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        if (CompletedWithDependentProcessors)
        {
            await CurrentCallingProcessorInCaseQueueIsEmpty.ProcessNextAsync(inputData);
            return;
        }


        await ProcessLogic(inputData);


        await _eventLoggingService.LogEvent($"PS: {this.ProcessorId} | ItemId: {this.InputId}, {this.ProcessorId}");

        var nextInQueProcessor = GetNextProcessor();

        if (nextInQueProcessor == null)
        {
            await HandleProcessorCompletion();
        }

        CurrentCallingProcessorInCaseQueueIsEmpty = nextInQueProcessor;
    }


    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        // if (ProcessorsDepended == null)
        // {
        //     ProcessorsDepended = new Queue<IProcessor<TInput>>();
        // }
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    private IProcessor<TInput> GetNextProcessor()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        return proc;
    }

    private async Task HandleProcessorCompletion()
    {
        await _eventLoggingService.LogEvent(
            $"PS: {this.ProcessorId}  HandleProcessorCompletion| ItemId: {this.InputId}, {this.ProcessorId}");
        CompletedWithDependentProcessors = true;
        SetUpNextTrackProcessor();
    }

    private void SetUpNextTrackProcessor()
    {
        this.CompletedWithDependentProcessors = false;
        this.CurrentCallingProcessorInCaseQueueIsEmpty = this;
    }

    #endregion
}