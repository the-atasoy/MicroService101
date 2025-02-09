using AutoMapper;
using CommandService.Data.Dto.Command;
using CommandService.Data.Entity;

namespace CommandService.Data.Profiles;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<CommandCreateDto, Command>();
    }
}