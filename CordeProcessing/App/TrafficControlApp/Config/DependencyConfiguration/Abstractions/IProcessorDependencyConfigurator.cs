namespace TrafficControlApp.Config.DependencyConfiguration.Abstractions;

public interface IProcessorDependencyConfigurator
{
    void ConfigureDependencies();
}

class ProcessorDependencyConfigurator : IProcessorDependencyConfigurator
{
    public void ConfigureDependencies()
    {
        throw new NotImplementedException();
    }
}