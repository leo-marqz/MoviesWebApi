using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getAllGenres")]
        public async Task<ActionResult<List<DisplayGenre>>> Get()
        {
            var genres = await context.Genres.ToListAsync();
            var dtos = mapper.Map<List<DisplayGenre>>(genres);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "getGenre")]
        public async Task<ActionResult<DisplayGenre>> Get(int id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            return mapper.Map<DisplayGenre>(genre);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateGenre createGenre)
        {
            var genre = mapper.Map<Genre>(createGenre);
            context.Add(genre);
            await context.SaveChangesAsync();
            var result = mapper.Map<DisplayGenre>(genre);
            return new CreatedAtRouteResult("getGenre", new { id=result.Id }, result);
        }

        [HttpPut("{id:int}", Name = "updateGenre")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateGenre updateGenre)
        {
            var genre = mapper.Map<Genre>(updateGenre);
            genre.Id = id;
            context.Entry(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "deleteGenre")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var exists = await context.Genres.AnyAsync(x => x.Id == id);
            if(!exists)
            {
                return NotFound();
            }
            context.Remove(new Genre { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
