using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Helpers;
using MoviesWebApi.Migrations;
using MoviesWebApi.Services;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string container = "authors";

        public AuthorsController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
            : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }


        [HttpGet(Name = "getAuthors")]
        public async Task<ActionResult<List<DisplayAuthor>>> Get([FromQuery] Pagination pagination)
        {
            return await Get<Author, DisplayAuthor>(pagination);
        }

        [HttpGet("{id:int}", Name = "getAuthor")]
        public async Task<ActionResult<DisplayAuthor>> Get([FromRoute] int id)
        {
            return await Get<Author, DisplayAuthor>(id);
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

        [HttpPut("{id:int}", Name = "updateAuthor")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromForm] UpdateAuthor updateAuthor)
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

        [HttpPatch("{id:int}", Name = "patchAuthor")]
        public async Task<ActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<PatchAuthor> jsonPatchDocument)
        {
            return await Patch<Author, PatchAuthor>(id, jsonPatchDocument);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            return await Delete<Author>(id);
        }
     
    }
}
