using DataFlowProducerConsumer.Config;
using DataFlowProducerConsumer.Consumers;
using DataFlowProducerConsumer.Consumers.Abstractions;
using DataFlowProducerConsumer.Models;
using DataFlowProducerConsumer.Processors;
using DataFlowProducerConsumer.Producers;
using DataFlowProducerConsumer.Producers.Abstraction;
using Microsoft.Extensions.Configuration;

namespace DataFlowProducerConsumer.Root.Abstractions;

public abstract class StartupConfigurator
{
    public void Configure()
    {
        CreateConfiguration();
        ConfigureProducers();
        ConfigureConsumers();
        ConfigureDependentProcessors();
        
    }

    protected abstract void CreateConfiguration();

    protected abstract void ConfigureProducers();

    protected abstract void ConfigureConsumers();

    protected abstract void ConfigureDependentProcessors();
}

class StartupConfiguratorImpl : StartupConfigurator
{
    private ApplicationConfiguration applicationConfiguration;
    private ITrackConsumer _trackConsumer;
    private ITrackProducer _trackProducer;
    private ITrackDevice _trackDevice;
    private IProcessor<Track> _RootProcessor;
    
    
    protected override void CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false);
        // .(Directory.GetCurrentDirectory())
        // .AddJsonFile("config.json", optional: false);

        IConfiguration config = builder.Build();
        //Create AppConfiguration
        //todo Make it deserialized from IConfiguration
        applicationConfiguration = new ApplicationConfiguration()
        {
            BoundedCapacity = 100,
            ProduceSpeed = TimeSpan.FromSeconds(0.5),
            MaxParallelConsumeCount = 4,
            PropagateCompletion = true,
            VehicleTypeAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleColorAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleSeasonAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleTrafficAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleDangerAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            },
            VehicleMarkAnalyseConfig = new()
            {
                TimeForAnalyse = TimeSpan.FromSeconds(2),
            }
        };
    }

    protected override void ConfigureProducers()
    {
        _trackProducer = new TrackProducer(_trackDevice, applicationConfiguration);
    }

    protected override void ConfigureConsumers()
    {
        _trackConsumer = new TrackConsumer(applicationConfiguration, _RootProcessor);
    }

    protected override void ConfigureDependentProcessors()
    {
        //TypeDependant
        _vehicleTypeProcessor.AddDependentProcessor(_vehicleMarkProcessor);
        _vehicleTypeProcessor.AddDependentProcessor(_vehicleDangerProcessor);
        //MarkDependant
        _vehicleMarkProcessor.AddDependentProcessor(_vehicleColorProcessor);
        _vehicleMarkProcessor.AddDependentProcessor(_vehicleSeasonProcessor);
        _vehicleMarkProcessor.AddDependentProcessor(_vehicleTrafficProcessor);
    }
}