using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Migrations;
using MoviesWebApi.Services;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "authors";

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
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
            if(createAuthor.Photo != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await createAuthor.Photo.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createAuthor.Photo.FileName);
                    author.Photo = await fileStorage
                        .SaveFile(content, extension, container, createAuthor.Photo.ContentType);
                }
            }
            context.Add(author);
            await context.SaveChangesAsync();
            return new CreatedAtRouteResult("getAuthor", new { id = author.Id }, author);
        }

        [HttpPut("id:int", Name = "updateAuthor")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] UpdateAuthor updateAuthor)
        {
            //var exist = await context.Authors.AnyAsync(x => x.Id == id);
            var authorDB = await context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if(authorDB is null) return NotFound();
            authorDB = mapper.Map(updateAuthor, authorDB);
            if (updateAuthor.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await updateAuthor.Photo.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(updateAuthor.Photo.FileName);
                    authorDB.Photo = await fileStorage
                        .EditFile(content, extension, container, authorDB.Photo, updateAuthor.Photo.ContentType);
                }
            }
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
