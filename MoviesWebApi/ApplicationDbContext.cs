using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Entities;

namespace MoviesWebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Genre> Genres { get; set; }
    }
}
