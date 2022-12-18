using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Migrations;
using MoviesWebApi.Services;
using System.ComponentModel;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "movies";

        public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet(Name = "getMovies")]
        public async Task<ActionResult<List<DisplayMovie>>> Get()
        {
            var movies = await context.Movies.ToListAsync();
            return mapper.Map<List<DisplayMovie>>(movies);
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<DisplayMovie>> Get([FromRoute] int id)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie is null) return NotFound();
            return mapper.Map<DisplayMovie>(movie);
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
            context.Add(movie);
            await context.SaveChangesAsync();
            var movieDTO = mapper.Map<DisplayMovie>(movie);
            return new CreatedAtRouteResult("getMovie", new {id=movie.Id }, movieDTO);
        }

        [HttpPut("{id:int}", Name = "updateMovie")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromForm] UpdateMovie updateMovie)
        {
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
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
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "patchMovie")]
        public async Task<ActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<PatchMovie> jsonPatchDocument)
        {
            if (jsonPatchDocument == null) return BadRequest();
            var movieDB = await context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movieDB is null) return NotFound();
            var patchMovie = mapper.Map<PatchMovie>(movieDB);
            jsonPatchDocument.ApplyTo(patchMovie, ModelState);
            var isValid = TryValidateModel(patchMovie);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(patchMovie, movieDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var exist = await context.Movies.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }
            context.Remove(new Movie { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
