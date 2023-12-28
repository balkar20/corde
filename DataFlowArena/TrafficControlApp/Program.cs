using TrafficControlApp.Root;

var startUpConfigurator = new TrafficControlStartupConfigurator();

try
{
    await startUpConfigurator.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}