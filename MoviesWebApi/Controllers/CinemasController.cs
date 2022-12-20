using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/cinemas")]
    public class CinemasController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CinemasController(ApplicationDbContext context, IMapper mapper)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getCinemas")]
        public async Task<ActionResult<List<DisplayCinema>>> Get()
        {
            return await Get<Cinema, DisplayCinema>();
        }

        [HttpGet("{id:int}", Name = "getCinema")]
        public async Task<ActionResult<DisplayCinema>> Get([FromRoute] int id)
        {
            return await Get<Cinema, DisplayCinema>(id);
        }

        [HttpPost(Name = "createCinema")]
        public async Task<ActionResult> Post([FromBody] CreateCinema createCinema)
        {
            return await Post<CreateCinema, Cinema, DisplayCinema>(createCinema, "getCinema");
        }

        [HttpPut("{id:int}", Name = "updateCinema")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] UpdateCinema updateCinema)
        {
            return await Put<UpdateCinema, Cinema>(id, updateCinema);
        }

        [HttpDelete("{id:int}", Name = "deleteCinema")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            return await Delete<Cinema>(id);
        }

    }
}
