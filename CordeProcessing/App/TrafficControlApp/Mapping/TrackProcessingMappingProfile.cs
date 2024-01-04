using AutoMapper;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;

namespace TrafficControlApp.Mapping;

public class TrackProcessingMappingProfile: Profile
{
    public TrackProcessingMappingProfile()
    {
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
    }
    
}