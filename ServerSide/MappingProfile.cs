using AutoMapper;
using ChatApp.Entities;
using ChatApp.Models;
using ServerSide.Models;

namespace ChatApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserDTO, User>();
            CreateMap<User, RegisterUserDTO>();
            CreateMap<MessageDTO, Message>();
            CreateMap<Message, SentMessageDTO>();
            CreateMap<IEnumerable<Message>, IEnumerable<MessageDTO>>();
        }
    }
}
