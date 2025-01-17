using AutoMapper;

namespace CommandService.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Models.Platform, Dtos.Platform.PlatformReadDto>();
    }
}