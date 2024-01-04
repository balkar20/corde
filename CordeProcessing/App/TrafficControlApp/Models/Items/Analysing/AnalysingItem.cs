using TrafficControlApp.Models.Items.Base;

namespace TrafficControlApp.Models.Items.Analysing;

public class AnalysingItem: ApplicationItem<string>
{
    

}

public class TypeAnalysingItem : AnalysingItem
{
    public string VehicleMark { get; set; }
    
    public string VehicleNumber { get; set; }
    
    public string VehicleModel { get; set; }
    
    public string VehicleColor { get; set; }
    
    public VehicleType VehicleType { get; set; }
}