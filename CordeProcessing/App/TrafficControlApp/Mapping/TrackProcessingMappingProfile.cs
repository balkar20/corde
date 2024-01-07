using AutoMapper;
using TrafficControlApp.Models;
using TrafficControlApp.Models.Items.Analysing;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;
using TrafficControlApp.RandomGeneration;

namespace TrafficControlApp.Mapping;

public class TrackProcessingMappingProfile: Profile
{
    public TrackProcessingMappingProfile()
    {
        var generator = new RandomAnalyseItemGenerator();
        CreateMap<VehicleTypeProcessionResult, TypeAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleSeasonProcessionResult, SeasonAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleTrafficProcessionResult, TrafficAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleDangerProcessionResult, DangerAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleColorProcessionResult, ColorAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleMarkProcessionResult, MarkAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        
        CreateMap<Track, TypeAnalysingItem>()
            // .ForMember(m => m.VehicleNumber, expression => expression.MapFrom(j => j.VehicleNumber))
            .ForMember(m => m.VehicleNumber, expression => expression.MapFrom(j => generator.GetRandomStringStartsWith("TypeAnalysingItem Number: ")))
            .ForMember(m => m.VehicleModel, expression => expression.MapFrom(j => generator.GetRandomStringStartsWith("TypeAnalysingItem Model: ")))
            .ForMember(m => m.VehicleMark, expression => expression.MapFrom(j => generator.GetRandomStringStartsWith("TypeAnalysingItem VehicleMark: ")))
            .ForMember(m => m.VehicleColor, expression => expression.MapFrom(j => generator.GetRandomStringStartsWith("TypeAnalysingItem VehicleColor: ")))
            .ForMember(m => m.VehicleType, expression => expression.MapFrom(j => VehicleType.Lightweight))
            .ReverseMap();
        
            // (o => o.MapFrom(i => new  RandomAnalyseItemGenerator().GenerateRandomColorAnalysingResultFromTrack(i)));
        
        // CreateMap<VehicleSeasonProcessionResult, SeasonAnalyseResult>()
        //     .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
        //     .ReverseMap();
        // CreateMap<VehicleTrafficProcessionResult, TrafficAnalyseResult>()
        //     .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
        //     .ReverseMap();
        // CreateMap<VehicleDangerProcessionResult, DangerAnalyseResult>()
        //     .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
        //     .ReverseMap();
        // CreateMap<VehicleColorProcessionResult, ColorAnalyseResult>()
        //     .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
        //     .ReverseMap();
        // CreateMap<VehicleMarkProcessionResult, MarkAnalyseResult>()
        //     .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
        //     .ReverseMap();
    }
    
}