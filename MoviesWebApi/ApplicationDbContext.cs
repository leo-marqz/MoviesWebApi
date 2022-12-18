using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Entities;

namespace MoviesWebApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesAuthors>().HasKey(x => new { x.AuthorId, x.MovieId });
            modelBuilder.Entity<MoviesGenres>().HasKey(x=> new {x.MovieId, x.GenreId});
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesAuthors> MoviesAuthors { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
    }
}
