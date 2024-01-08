using System.Collections.Frozen;
using TrafficControlApp.Models.Items.Base;
using TrafficControlApp.Models.Results.Procession.Abstractions;
using TrafficControlApp.Services.Events.Abstractions;

namespace TrafficControlApp.Processors.Abstractions;

public abstract class Processor<TInput, TProcessionResult>(
        IEventLoggingService? eventLoggingService)
    : IProcessor<TInput>
where TInput: ApplicationItem<string>
where TProcessionResult: IProcessionResult
{
    #region private fields
    
    private readonly IEventLoggingService? EventLoggingService = eventLoggingService;

    private Queue<IProcessor<TInput>> ProcessorsDepended = new ();
    
    protected readonly Queue<IProcessor<TInput>> ProcessorsDependedCopy = new ();

    #endregion

    #region Public Properties

    public string ProcessorId  => Guid.NewGuid().ToString();
    
    public string InputId { get; set; }
    
    public bool CompletedProcessing { get; set; }
    
    public IProcessor<TInput> ProcessorFromDependentQue { get; set; }

    #endregion
    
    #region Events

    public event IProcessor<TInput>.NotifyNestedProcessingCompleted NestedProcessingCompleted;
        
    #endregion

    #region Protected Abstract Methods

    protected abstract Task<IProcessionResult> ProcessLogic(TInput inputData);
    
    
    protected abstract Task SetProcessionResult(TProcessionResult result);

    #endregion

    #region Public Methods

    public async Task ProcessNextAsync(TInput inputData)
    {
        try
        {
            if (this.InputId != inputData.ItemId)
            {
                EventLoggingService?.LogEvent($"ProcessNextAsync New item Id: {inputData.ItemId}");
                this.InputId = inputData.ItemId;
            } 
            if (ProcessorFromDependentQue != null)
            {
                await ProcessorFromDependentQue.ProcessNextAsync(inputData);
                
                SetNextProcessorFromDependent(ProcessorFromDependentQue);
                return;
            }
            
            
            var processionResult = await ProcessLogic(inputData);
            SetNextProcessor();
            await EventLoggingService.LogEvent($"PS: {this.ProcessorId} | ItemId: {this.InputId}, {this.ProcessorId}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void SetNextProcessorFromDependent(IProcessor<TInput> processorFromDependentQue)
    {
        if (processorFromDependentQue.CompletedProcessing)
        {
            ProcessorFromDependentQue = GetNextProcessorFromDependants();
            if (CompletedProcessing)
            {
                NestedProcessingCompleted?.Invoke();
            }
        }
    }

    public void AddDependentProcessor(IProcessor<TInput> dependentProcessor)
    {
        ProcessorsDepended.Enqueue(dependentProcessor);
    }

    #endregion


    #region Private Methods

    protected IProcessor<TInput> GetNextProcessorFromDependants()
    {
        ProcessorsDepended.TryDequeue(out IProcessor<TInput>? proc);
        if (proc == null)
        {
            CompletedProcessing = true;
        }

        return proc;
    }
    
    protected void SetNextProcessor()
    {
        ProcessorFromDependentQue = GetNextProcessorFromDependants();
    }

    public void SetDependents(Queue<IProcessor<TInput>> dependents)
    {
        ProcessorsDepended = dependents;
    }

    #endregion
}