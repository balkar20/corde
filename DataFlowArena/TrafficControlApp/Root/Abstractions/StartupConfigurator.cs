namespace TrafficControlApp.Root.Abstractions;

public abstract class StartupConfigurator
{
    
    public void Configure()
    {
        CreateConfiguration();
        ConfigureProducers();
        ConfigureConsumers();
        ConfigureDependentProcessors();
        // ConfigureSharedMemory();
    }

    public abstract Task Run();
    
    

    protected abstract void CreateConfiguration();

    protected abstract void ConfigureProducers();

    protected abstract void ConfigureConsumers();
    protected abstract void ConfigureMapping();

    // protected abstract void ConfigureSharedMemory();
    
    protected abstract void ConfigureDependentProcessors();
}