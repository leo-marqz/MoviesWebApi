using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet(Name = "getAuthors")]
        public async Task<ActionResult<List<DisplayAuthor>>> Get()
        {
            var authors = await context.Authors.ToListAsync();
            return mapper.Map<List<DisplayAuthor>>(authors);
        }

        [HttpGet("{id:int}", Name = "getAuthor")]
        public async Task<ActionResult<DisplayAuthor>> Get([FromRoute] int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if(author is null)
            {
                return NotFound();
            }
            return mapper.Map<DisplayAuthor>(author);
        }

        [HttpPost(Name = "createAuthor")]
        public async Task<ActionResult> Post([FromForm] CreateAuthor createAuthor)
        {
            var author = mapper.Map<Author>(createAuthor);
            context.Add(author);
            await context.SaveChangesAsync();
            return new CreatedAtRouteResult("getAuthor", new { id = author.Id }, author);
        }

        [HttpPut("id:int", Name = "updateAuthor")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] UpdateAuthor updateAuthor)
        {
            var exist = await context.Authors.AnyAsync(x => x.Id == id);
            if(!exist)
            {
                return NotFound();
            }
            var author = mapper.Map<Author>(updateAuthor);
            author.Id = id;
            context.Entry(author).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            var exist = await context.Authors.AnyAsync(x => x.Id == id);
            if(!exist)
            {
                return NotFound();
            }
            context.Remove(new Author { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
     
    }
}
