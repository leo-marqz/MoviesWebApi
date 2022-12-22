using Microsoft.AspNetCore.Mvc;
using MoviesWebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests.UnitTests
{
    [TestClass]
    public class GenresControllerTests : TestBase
    {
        [TestMethod]
        public async Task GetAllGenres()
        {
            //Preparation
            var DbName = Guid.NewGuid().ToString();
            var context = CreateContext(DbName);
            var mapper = ConfigureAutoMapper();
            context.Genres.Add(new Entities.Genre { Name = "Genero 1" });
            context.Genres.Add(new Entities.Genre { Name = "Genero 2" });
            await context.SaveChangesAsync();

            var context2 = CreateContext(DbName);

            //Test
            var controller = new GenresController(context2, mapper);
            var response = await controller.Get();

            //Verification
            var genres = response.Value;
            Assert.AreEqual(2, genres.Count);

        }

        [TestMethod]
        public async Task GetGenreByIdNotExists()
        {
            //Preparation
            var DbName = Guid.NewGuid().ToString();
            var context = CreateContext(DbName);
            var mapper = ConfigureAutoMapper();
            
            //Test
            var controller = new GenresController(context, mapper);
            var response = await controller.Get(1);
            var result = response.Result as StatusCodeResult;

            //Verification
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
