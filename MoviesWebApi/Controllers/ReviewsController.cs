using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.DTOs;
using MoviesWebApi.Entities;
using MoviesWebApi.Helpers;
using MoviesWebApi.Migrations;
using System.Security.Claims;

namespace MoviesWebApi.Controllers
{
    [ApiController]
    [Route("api/movies/{movieId:int}/reviews")]
    [ServiceFilter(typeof(MovieExistsAttribute))]
    public class ReviewsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ReviewsController(ApplicationDbContext context, IMapper mapper)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getReviews")]
        public async Task<ActionResult<List<DisplayReview>>> Get(int movieId, [FromQuery] Pagination pagination)
        {
            var queryable = context.Reviews.Include(x => x.User).AsQueryable();
            queryable = queryable.Where(x => x.MovieId == movieId);
            return await Get<Review, DisplayReview>(pagination, queryable);
        }

        [HttpPost(Name = "createReview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int movieId, [FromBody] CreateReview createReview)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var reviewExists = await context.Reviews.AnyAsync(x=>x.MovieId == movieId && x.UserId == userId);
            if(reviewExists)
            {
                return BadRequest("Ya existe una review de esta pelicula");
            }
            var review = mapper.Map<Review>(createReview);
            review.MovieId = movieId;
            review.UserId = userId;
            context.Add(review);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{reviewId:int}" ,Name = "updateReview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int movieId, int reviewId, [FromBody] UpdateReview updateReview)
        {
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null) return NotFound();
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if(reviewDB.UserId != userId)
            {
                return Forbid();
            }

            reviewDB = mapper.Map(updateReview, reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}", Name = "deleteReview")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int reviewId)
        {
            var reviewDB = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId);
            if (reviewDB == null) return NotFound();
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (reviewDB.UserId != userId)
            {
                return Forbid();
            }

            context.Remove(reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
