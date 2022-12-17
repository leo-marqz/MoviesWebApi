using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace MoviesWebApi.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationParameters<T>(
            this HttpContext httpContext, 
            IQueryable<T> queryable, 
            int recordsPerPage
            )
        {
            double amount = await queryable.CountAsync();
            double numberOfPages = Math.Ceiling(amount / recordsPerPage);
            httpContext.Response.Headers.Add("pages", numberOfPages.ToString());
        }
    }
}
