namespace TrafficControlApp.Models;

public class Vehicle
{
    public string VehicleNumber { get; set; }
    
    public string VehicleMark { get; set; }
    
    public string VehicleModel { get; set; }
    public string VehicleColor { get; set; }
    
    public VehicleType VehicleType { get; set; }
}