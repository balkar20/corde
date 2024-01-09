using TrafficControlApp.Exceptions;
using TrafficControlApp.Exceptions.Abstractions;
using TrafficControlApp.Root;

var startUpConfigurator = new TrafficControlStartupConfigurator();

try
{
    await startUpConfigurator.Run();
    Console.ReadLine();
}
catch (ProcessingItemCreationException ex)
{
    Console.WriteLine(ex);
    Console.WriteLine($"With Data: \n  {ex.Data}");

    throw;
}
catch (ProcessingException processingException)
{
    Console.WriteLine(processingException);
    throw;
}
catch (AnalysingException analysingException)
{
    Console.WriteLine(analysingException);
    throw;
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}