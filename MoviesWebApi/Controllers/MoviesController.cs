using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Helpers;
using MoviesWebApi.Migrations;
using MoviesWebApi.Services;
using System.ComponentModel;
using System.Linq.Dynamic.Core;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly ILogger<MoviesController> logger;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage, ILogger<MoviesController> logger)
        :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
            this.logger = logger;
        }

        [HttpGet(Name = "getMovies")]
        public async Task<ActionResult<TopMoviesList>> Get()
        {
            var top = 5;
            var today = DateTime.Today;
            var nextReleases = await context.Movies
                .Where(x => x.ReleaseDate > today)
                .OrderBy(x => x.ReleaseDate)
                .Take(top)
                .ToListAsync();
            var inCinemas = await context.Movies
                .Where(x => x.InCinemas)
                .Take(top)
                .ToListAsync();

            var result = new TopMoviesList();
            result.FutureReleases = mapper.Map<List<DisplayMovie>>(nextReleases);
            result.InCinemas = mapper.Map<List<DisplayMovie>>(inCinemas);
            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<DisplayMovie>>> Filter([FromQuery] MovieFilter movieFilter)
        {
            var moviesQueryable = context.Movies.AsQueryable();
            if(!string.IsNullOrEmpty(movieFilter.Title))
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.Title.Contains(movieFilter.Title));
            }
            if(movieFilter.InCinemas)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.InCinemas);
            }
            if(movieFilter.NextReleases)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.ReleaseDate > DateTime.Today);
            }
            if(movieFilter.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(movieFilter.GenreId));
            }
            if(!string.IsNullOrEmpty(movieFilter.FieldSort))
            {
                if(movieFilter.FieldSort == "title")
                {
                    var orderType = movieFilter.AscendingOrder ? "ascending" : "descending";
                    try
                    {
                        moviesQueryable = moviesQueryable.OrderBy($"{movieFilter.FieldSort} {orderType}");
                    }catch(Exception ex)
                    {
                        logger.LogError(ex.Message, ex);
                    }
                }
            }
            await HttpContext
                .InsertPaginationParameters(moviesQueryable, movieFilter.recordsPerPage);
            var movies = await moviesQueryable.Page(movieFilter.Pagination).ToListAsync();
            return mapper.Map<List<DisplayMovie>>(movies);
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<DetailsOfMovies>> Get([FromRoute] int id)
        {
            var movie = await context.Movies
                .Include(x=>x.MoviesAuthors).ThenInclude(x=> x.Author)
                .Include(x=>x.MoviesGenres).ThenInclude(x=> x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (movie is null) return NotFound();
            movie.MoviesAuthors = movie.MoviesAuthors.OrderBy(x => x.Order).ToList();
            return mapper.Map<DetailsOfMovies>(movie);
        }

        [HttpPost(Name = "createMovie")]
        public async Task<ActionResult<DisplayMovie>> Post([FromForm] CreateMovie createMovie)
        {
            var movie = mapper.Map<Movie>(createMovie);
            if (createMovie.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createMovie.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createMovie.Poster.FileName);
                    movie.Poster = await fileStorage
                        .SaveFile(content, extension, container, createMovie.Poster.ContentType);
                }
            }
            AssignAuthorsOrderInTheMovie(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<DisplayMovie>(movie);
            return new CreatedAtRouteResult("getMovie", new {id=movie.Id }, movieDTO);
        }

        [HttpPut("{id:int}", Name = "updateMovie")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromForm] UpdateMovie updateMovie)
        {
            var movieDB = await context.Movies
                .Include(x=>x.MoviesGenres)
                .Include(x=>x.MoviesAuthors)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB is null) return NotFound();
            movieDB = mapper.Map(updateMovie, movieDB);
            if (updateMovie.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await updateMovie.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(updateMovie.Poster.FileName);
                    movieDB.Poster = await fileStorage
                        .EditFile(content, extension, container, movieDB.Poster, updateMovie.Poster.ContentType);
                }
            }
            AssignAuthorsOrderInTheMovie(movieDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "patchMovie")]
        public async Task<ActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<PatchMovie> jsonPatchDocument)
        {
            return await Patch<Movie, PatchMovie>(id, jsonPatchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            return await Delete<Movie>(id);
        }

        private void AssignAuthorsOrderInTheMovie(Movie movie)
        {
            if(movie.MoviesAuthors != null)
            {
                for (int i = 0; i < movie.MoviesAuthors.Count; i++)
                {
                    movie.MoviesAuthors[i].Order = i;
                }
            }
        }
    }
}
