using AutoMapper;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, DisplayGenre>()
                .ReverseMap();
            CreateMap<CreateGenre, Genre>();
            CreateMap<UpdateGenre, Genre>();

            CreateMap<Author, DisplayAuthor>()
                .ReverseMap();
            CreateMap<CreateAuthor, Author>();
            CreateMap<UpdateAuthor, Author>()
                .ForMember(x => x.Photo, options => options.Ignore());
            CreateMap<PatchAuthor, Author>()
                .ReverseMap();

            CreateMap<Movie, DisplayMovie>().ReverseMap();
            CreateMap<CreateMovie, Movie>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesAuthors, options => options.MapFrom(MapMoviesAuthors));
            CreateMap<UpdateMovie, Movie>()
                .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesAuthors, options => options.MapFrom(MapMoviesAuthors));
            CreateMap<Movie, PatchMovie>().ReverseMap();
            CreateMap<Movie, DetailsOfMovies>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapDetailsMoviesGenres))
                .ForMember(x => x.Authors, options => options.MapFrom(MapDetailsMoviesAuthors));
        }

        private List<DisplayGenre> MapDetailsMoviesGenres(Movie movie, DetailsOfMovies detailsOfMovies)
        {
            var result = new List<DisplayGenre>();
            if (movie.MoviesGenres == null) return result;
            foreach (var movieGenre in movie.MoviesGenres)
            {
                result.Add(new DisplayGenre { Id = movieGenre.GenreId, Name = movieGenre.Genre.Name }); 
            }
            return result;
        }

        private List<DetailsOfAuthor> MapDetailsMoviesAuthors(Movie movie, DetailsOfMovies detailsOfMovies)
        {
            var result = new List<DetailsOfAuthor>();
            if (movie.MoviesAuthors == null) return result;
            foreach (var movieAuthor in movie.MoviesAuthors)
            {
                result.Add(new DetailsOfAuthor
                {
                    AuthorId = movieAuthor.AuthorId,
                    Character = movieAuthor.Character,
                    Name = movieAuthor.Author.Name
                });
            }
            return result;
        }

        private List<MoviesGenres> MapMoviesGenres<T>(T movieDTO, Movie movie) 
            where T: CreateMovie
        {
            var result = new List<MoviesGenres>();
            if(movieDTO.GenresIds == null)
            {
                return result;
            }
            foreach (var id in movieDTO.GenresIds)
            {
                result.Add(new MoviesGenres { GenreId = id });
            }
            return result;
        }

        private List<MoviesAuthors> MapMoviesAuthors<T>(T movieDTO, Movie movie) 
            where T : CreateMovie
        {
            var result = new List<MoviesAuthors>();
            if (movieDTO.Authors == null)
            {
                return result;
            }
            foreach (var author in movieDTO.Authors)
            {
                result.Add(new MoviesAuthors { 
                    AuthorId = author.AuthorId, Character = author.Character
                });
            }
            return result;
        }

    }
}
