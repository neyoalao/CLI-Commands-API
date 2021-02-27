using AutoMapper;
using CommandsAPI.Dtos;
using CommandsAPI.Models;

namespace CommandsAPI.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //Source -> Target
            CreateMap<Commands, CommandReadDto>();
            CreateMap<CommandCreateDto, Commands>();
            CreateMap<CommandUpdateDto, Commands>();
            CreateMap<Commands, CommandUpdateDto>();
        }
    } 
}