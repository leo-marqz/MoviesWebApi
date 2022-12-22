using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Entities;
using System.Security.Claims;

namespace MoviesWebApi
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesAuthors>().HasKey(x => new { x.AuthorId, x.MovieId });
            modelBuilder.Entity<MoviesGenres>().HasKey(x=> new {x.MovieId, x.GenreId});
            modelBuilder.Entity<MoviesCinemas>().HasKey(x => new { x.MovieId, x.CinemaId });
            SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var roleAdminId = "4359b4b4-2b30-48cc-8e77-6ceabaa1794a";
            var userAdminId = "e8c76789-28be-4e42-baab-26a4e53cf1da";

            var roleAdmin = new IdentityRole
            {
                Id = roleAdminId,
                Name = "Admin",
                NormalizedName = "Admin"
            };
            var passwordHash = new PasswordHasher<IdentityUser>();
            var userNameAdmin = "leomarqz@gmail.com";
            var userAdmin = new IdentityUser
            {
                Id = userAdminId,
                UserName = userNameAdmin,
                NormalizedUserName = userNameAdmin,
                Email = userNameAdmin,
                NormalizedEmail = userNameAdmin,
                PasswordHash = passwordHash.HashPassword(null, "@Admin123456")
            };
            modelBuilder.Entity<IdentityUser>().HasData(userAdmin);
            modelBuilder.Entity<IdentityRole>().HasData(roleAdmin);
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasData(new IdentityUserClaim<string>()
                {
                    Id = 1,
                    ClaimType = ClaimTypes.Role,
                    UserId = userAdminId,
                    ClaimValue = "Admin"
                });

        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MoviesAuthors> MoviesAuthors { get; set; }
        public DbSet<MoviesGenres> MoviesGenres { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MoviesCinemas> MoviesCinemas { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
