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

            CreateMap<Author, DisplayAuthor>().ReverseMap();
            CreateMap<CreateAuthor, Author>();
            CreateMap<UpdateAuthor, Author>().ForMember(x => x.Photo, options => options.Ignore());
            CreateMap<PatchAuthor, Author>().ReverseMap();

            CreateMap<Movie, DisplayMovie>();
            CreateMap<CreateMovie, Movie>().ForMember(x => x.Poster, options => options.Ignore());
            CreateMap<UpdateMovie, Movie>();
            CreateMap<Movie, PatchMovie>().ReverseMap();
        }
    }
}
