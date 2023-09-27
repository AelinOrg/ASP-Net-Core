using AutoMapper;
using MovieAPI.Data.Dtos;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class CineProfile : Profile
{
    public CineProfile()
    {
        CreateMap<CreateCineDto, Cine>();
        CreateMap<UpdateCineDto, Cine>();
        CreateMap<Cine, UpdateMovieDto>();
    }
}