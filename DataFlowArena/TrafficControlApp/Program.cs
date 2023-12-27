using TrafficControlApp.Root;

var startUpConfigurator = new TrafficControlStartupConfigurator();
await startUpConfigurator.Run();