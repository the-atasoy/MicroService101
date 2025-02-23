using AutoMapper;
using CommandService.Data.Dto.Platform;
using CommandService.Data.Entity;

namespace CommandService.Data.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformPublishedDto, Platform>().ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

    }
}