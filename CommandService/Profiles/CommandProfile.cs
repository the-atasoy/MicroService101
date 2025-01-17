using AutoMapper;

namespace CommandService.Profiles;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<Models.Command, Dtos.Command.CommandReadDto>();
        CreateMap<Dtos.Command.CommandCreateDto, Models.Command>();
    }
}