using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors;
using DataFlowProducerConsumer.Processors.Abstractions;

namespace DataFlowProducerConsumer.PrsStructure;

// public class ProcessorPool<TOutput>:Processor<Track> where TOutput : IProcessResult, new()
// {
//     public Processor<Track> CurrentProcessor { get; set; }
//
//     // public List<IProcessor<Track, IProcessResult>> Processors { get; set; }
//
//     // public async Task<IProcessResult> ProcessNext(Track inputData)
//     // {
//     //     if (CurrentProcessor.Completed)//Maybe && canRun??
//     //     {
//     //         // await CurrentProcessor.ProcessAsync(inputData);
//     //         await CurrentProcessor.ProcessAsync(inputData);
//     //     }
//     //
//     //     foreach (var processor in ProcessoresDepended)
//     //     {
//     //         return await processor.ProcessAsync(inputData);
//     //         // CurrentProcessor.
//     //     }
//     // }
//
//     public override async Task<IProcessResult> ProcessLogic(Track inputData)
//     {
//         if (!CurrentProcessor.Completed)//Maybe && canRun??
//         {
//             // await CurrentProcessor.ProcessAsync(inputData);
//             await CurrentProcessor.ProcessLogic(inputData);
//             CurrentProcessor.Completed = true;
//             // CurrentProcessor = ProcessorsDepended.
//         }
//
//         // return (IProcessResult)(await ProcessorsDepended.First.Value.ProcessAsync(inputData));
//         // foreach (var processor in ProcessorsDepended)
//         // {
//         //     return (TOutput)(await processor.ProcessAsync(inputData));
//         //     // CurrentProcessor.
//         // }
//     }
// }