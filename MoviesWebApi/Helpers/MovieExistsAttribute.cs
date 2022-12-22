using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MoviesWebApi.Helpers
{
    public class MovieExistsAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext DbContext;

        public MovieExistsAttribute(ApplicationDbContext DbContext)
        {
            this.DbContext = DbContext;
        }
        public async Task OnResourceExecutionAsync(
            ResourceExecutingContext context, 
            ResourceExecutionDelegate next
            )
        {
            var movieIdObj = context.HttpContext.Request.RouteValues["movieId"];
            if (movieIdObj == null) return;
            var movieId = int.Parse(movieIdObj.ToString());
            var movieExists = await DbContext.Movies.AnyAsync(x => x.Id == movieId);
            if (!movieExists)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
