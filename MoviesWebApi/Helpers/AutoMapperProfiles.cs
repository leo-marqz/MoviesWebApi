using AutoMapper;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, DisplayGenre>().ReverseMap();
            CreateMap<CreateGenre, Genre>();
            CreateMap<UpdateGenre, Genre>();
        }
    }
}
