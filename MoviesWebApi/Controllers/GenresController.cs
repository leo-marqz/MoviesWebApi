using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenresController(ApplicationDbContext context, IMapper mapper)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getAllGenres")]
        public async Task<ActionResult<List<DisplayGenre>>> Get()
        {
            return await Get<Genre, DisplayGenre>();
        }

        [HttpGet("{id:int}", Name = "getGenre")]
        public async Task<ActionResult<DisplayGenre>> Get(int id)
        {
            return await Get<Genre, DisplayGenre>(id: id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateGenre createGenre)
        {
            return await Post<CreateGenre, Genre, DisplayGenre>(createGenre, "getGenre");
        }

        [HttpPut("{id:int}", Name = "updateGenre")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateGenre updateGenre)
        {
            return await Put<UpdateGenre, Genre>(id, updateGenre);
        }

        [HttpDelete("{id:int}", Name = "deleteGenre")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            return await Delete<Genre>(id);
        }
    }
}
