using AutoMapper;
using PlatformService.Data.Dto.Platform;
using PlatformService.Data.Entity;

namespace PlatformService.Data.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformCreateDto, PlatformReadDto>();
        CreateMap<PlatformCreateDto, PlatformPublishedDto>();
    }
}