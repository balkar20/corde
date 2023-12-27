// // See https://aka.ms/new-console-template for more information
//
// using Microsoft.Extensions.Configuration;
// using TrafficControlApp;
// using TrafficControlApp.Config;
// using TrafficControlApp.Consumers;
// using TrafficControlApp.Consumers.Abstractions;
// using TrafficControlApp.Models;
// using TrafficControlApp.Processors;
// using TrafficControlApp.Processors.Abstractions;
// using TrafficControlApp.Producers;
// using TrafficControlApp.Producers.Abstraction;
// using TrafficControlApp.Services;
// using TrafficControlApp.Services.Storage;
//
//
// var builder = new ConfigurationBuilder();
// builder.AddJsonFile("appsettings.json", optional: false);
// // .(Directory.GetCurrentDirectory())
// // .AddJsonFile("config.json", optional: false);
//
// IConfiguration config = builder.Build();
// //Create AppConfiguration
// //todo Make it deserialized from IConfiguration
// ApplicationConfiguration applicationConfiguration = CreateConfiguration();
//
//
//
// //SharedMemory
// ISharedMemoryVehicleService sharedMemoryVehicleService= new SharedMemoryVehicleService();
//
// //TrackDevice
// ITrackDevice trackDevice = new TrackDevice();
//
// //Processors
// Processor<Track> _vehicleTypeProcessor = new VehicleTypeProcessor(sharedMemoryVehicleService);
// Processor<Track> _vehicleMarkProcessor = new VehicleMarkProcessor(sharedMemoryVehicleService);
// Processor<Track> _vehicleColorProcessor = new VehicleColorProcessor(sharedMemoryVehicleService);
// Processor<Track> _vehicleSeasonProcessor = new VehicleSeasonProcessor(sharedMemoryVehicleService);
// Processor<Track> _vehicleTrafficProcessor = new VehicleTrafficProcessor(sharedMemoryVehicleService);
// Processor<Track> _vehicleDangerProcessor = new VehicleDangerProcessor(sharedMemoryVehicleService);
//
// //producers
// ITrackProducer _trackProducer = new TrackProducer(trackDevice, applicationConfiguration);
// ITrackConsumer _trackConsumer = new TrackConsumer(applicationConfiguration, _vehicleTypeProcessor);
//
// //TypeDependant
// _vehicleTypeProcessor.AddDependentProcessor(_vehicleMarkProcessor);
// _vehicleTypeProcessor.AddDependentProcessor(_vehicleDangerProcessor);
// //MarkDependant
// _vehicleMarkProcessor.AddDependentProcessor(_vehicleColorProcessor);
// _vehicleMarkProcessor.AddDependentProcessor(_vehicleSeasonProcessor);
// _vehicleMarkProcessor.AddDependentProcessor(_vehicleTrafficProcessor);
//
// await new TrafficFlowWorker(
//         trackDevice, 
//         _vehicleTypeProcessor, 
//         _trackProducer,
//         _trackConsumer)
//     .StartProcess();
// // var buffer = new BufferBlock<byte[]>();
// //
// // //Here we started execution and Got task
// // var consumerTask = ProducerConsumerArena.ConsumeAsync(buffer);
// //
// //
// // var typeProcessor = new VehicleTypeProcessor();
// // // typeProcessor.ProcessAsync();
// //
// // ProducerConsumerArena.Produce(buffer);
// //
// // var bytesProcessed = await consumerTask;
// //
// // Console.WriteLine($"Processed {bytesProcessed:#,#} bytes.");
//
// ApplicationConfiguration CreateConfiguration()
// {
//     var builder = new ConfigurationBuilder();
//     builder.AddJsonFile("appsettings.json", optional: false);
//     // .(Directory.GetCurrentDirectory())
//     // .AddJsonFile("config.json", optional: false);
//
//     IConfiguration config = builder.Build();
//     //Create AppConfiguration
//     //todo Make it deserialized from IConfiguration
//     return new ApplicationConfiguration()
//     {
//         BoundedCapacity = 100,
//         ProduceSpeed = TimeSpan.FromSeconds(0.5),
//         MaxParallelConsumeCount = 4,
//         PropagateCompletion = true,
//         VehicleTypeAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         },
//         VehicleColorAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         },
//         VehicleSeasonAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         },
//         VehicleTrafficAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         },
//         VehicleDangerAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         },
//         VehicleMarkAnalyseConfig = new()
//         {
//             TimeForAnalyse = TimeSpan.FromSeconds(2),
//         }
//     };
// }