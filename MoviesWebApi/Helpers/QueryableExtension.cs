using MoviesWebApi.DTOs;

namespace MoviesWebApi.Helpers
{
    public static class QueryableExtension
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, Pagination pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.recordsPerPage)
                .Take(pagination.recordsPerPage);
        }
    }
}
