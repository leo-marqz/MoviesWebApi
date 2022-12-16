using Microsoft.AspNetCore.Mvc;
using MoviesWebApi.DTOs;

namespace MoviesWebApi.Controllers
{
    public class AuthorsController : Controller
    {
        [HttpGet(Name = "getAuthors")]
        public async Task<ActionResult<List<DisplayAuthor>>> Get()
        {
            return Ok();
        }

        [HttpGet("{id:int}", Name = "getAuthor")]
        public async Task<ActionResult<DisplayAuthor>> Get([FromRoute] int id)
        {
            return Ok();
        }
    }
}
