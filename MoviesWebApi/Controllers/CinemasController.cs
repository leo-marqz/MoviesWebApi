using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using NetTopologySuite.Geometries;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/cinemas")]
    public class CinemasController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly GeometryFactory geometryFactory;

        public CinemasController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.geometryFactory = geometryFactory;
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

        [HttpGet("nearby", Name = "getCinemaNearby")]
        public async Task<ActionResult<List<NearestCinema>>> Nearby([FromQuery] NearCinemaFilter filter)
        {
            var userLocation = geometryFactory
                .CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));
            var cinemas = await context.Cinemas
                .OrderBy(x => x.Location.Distance(userLocation))
                .Where(x => x.Location.IsWithinDistance(userLocation, filter.DistanceInKMs * 1000))
                .Select(x => new NearestCinema()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Latitude = x.Location.Y,
                    Longitude = x.Location.X,
                    DistanceInMeters = Math.Round(x.Location.Distance(userLocation))
                }).ToListAsync();
            return cinemas;
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
