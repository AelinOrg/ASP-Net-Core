using API.Data.Dtos;
using API.Models;
using AutoMapper;

namespace API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<SignUpDto, User>()
            // Não usamos o `UserName`, mas é obrigatório, então mapeamos para o `Email`
            .ForMember(user => user.UserName, option => option.MapFrom(dto => dto.Email));
    }
}
