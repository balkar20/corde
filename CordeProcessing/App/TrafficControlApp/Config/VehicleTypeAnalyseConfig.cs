namespace TrafficControlApp.Config;

public class VehicleTypeAnalyseConfig: IVehicleAnalyseConfig
{
    public TimeSpan TimeForAnalyse { get; set; }
}