using AutoMapper;
using TrafficControlApp.Models.Results;
using TrafficControlApp.Models.Results.Analyse;

namespace TrafficControlApp.Mapping;

public class TrackProcessingMappingProfile: Profile
{
    public TrackProcessingMappingProfile()
    {
        CreateMap<VehicleTypeProcessResult, TypeAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleSeasonProcessResult, SeasonAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleTrafficProcessResult, TrafficAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleDangerProcessResult, DangerAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleColorProcessResult, ColorAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
        CreateMap<VehicleMarkProcessResult, MarkAnalyseResult>()
            .ForMember(m => m.Message , expression => expression.MapFrom(j => j.Message))
            .ReverseMap();
    }
    
}