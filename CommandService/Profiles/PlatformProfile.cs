using AutoMapper;
using CommandService.Dtos.Platform;
using CommandService.Models;

namespace CommandService.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformPublishedDto, Platform>().ForMember(dest => dest.ExternalId,
            opt => opt.MapFrom(src => src.Id));
    }
}