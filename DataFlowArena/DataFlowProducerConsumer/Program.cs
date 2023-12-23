// See https://aka.ms/new-console-template for more information

using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer;
using DataFlowProducerConsumer.Processors;

var buffer = new BufferBlock<byte[]>();
var consumerTask = ProducerConsumerArena.ConsumeAsync(buffer);


var typeProcessor = new VehicleTypeProcessor();
typeProcessor.ProcessAsync();
ProducerConsumerArena.Produce(buffer);

var bytesProcessed = await consumerTask;

Console.WriteLine($"Processed {bytesProcessed:#,#} bytes.");