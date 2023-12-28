using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Processors.Abstractions;
using TrafficControlApp.Root;

var startUpConfigurator = new TrafficControlStartupConfigurator();

try
{
    await startUpConfigurator.Run();
}
catch (ProcessingException<IProcessor<IProcessResult>> processingException)
{
    Console.WriteLine(processingException);
    throw;
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}