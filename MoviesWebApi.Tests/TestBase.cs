using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Helpers;
using NetTopologySuite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApi.Tests
{
    public class TestBase
    {
        //retorna un ApplicationDbContext con el cual podremos 
        //crear una base de datos por cada prueba sin perjudicar 
        //la base de datos original.
        protected ApplicationDbContext CreateContext(string DBName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(DBName).Options;
            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        //retorna una instancia de automapper con todas las configuraciones de nuestro 
        //automapper en MoviesWebApi
        protected IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }
    }
}
