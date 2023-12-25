// See https://aka.ms/new-console-template for more information

using System.Threading.Tasks.Dataflow;
using DataFlowProducerConsumer;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Models.Results;
using DataFlowProducerConsumer.Processors;
using DataFlowProducerConsumer.PrsStructure;
using DataFlowProducerConsumer.Services;

// await new ProducerConsumerArena2().Demonstrate();



ITrackDevice trackDevice = new TrackDevice();
ISharedMemoryVehicleService sharedMemoryVehicleService= new SharedMemoryVehicleService();


IProcessor<Track, VehicleTypeProcessResult> _vehickleTypeProcessor = new VehicleTypeProcessor(sharedMemoryVehicleService);
// IProcessor<Track, VehicleTypeProcessResult> _vehickleMarkProcessor = new VehicleMarkProcessor(sharedMemoryVehicleService);
ProcessorPool<PoolProcessResult> pool = new ProcessorPool<PoolProcessResult>();
pool.Processors.Add(_vehickleTypeProcessor);
await new ProducerConsumerArena2(trackDevice, pool).DemonstrateSec();
// var buffer = new BufferBlock<byte[]>();
//
// //Here we started execution and Got task
// var consumerTask = ProducerConsumerArena.ConsumeAsync(buffer);
//
//
// var typeProcessor = new VehicleTypeProcessor();
// // typeProcessor.ProcessAsync();
//
// ProducerConsumerArena.Produce(buffer);
//
// var bytesProcessed = await consumerTask;
//
// Console.WriteLine($"Processed {bytesProcessed:#,#} bytes.");