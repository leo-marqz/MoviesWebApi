using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoviesWebApi.Helpers;
using MoviesWebApi.Services;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            //services.AddTransient<IFileStorage, AzureFileStorage>();
            services.AddTransient<IFileStorage, LocalFileStorage>();
            services.AddHttpContextAccessor();
            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
            services.AddSingleton(provider =>
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));

                }).CreateMapper()
            );

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"), 
                    slqServerOptions => slqServerOptions.UseNetTopologySuite()
                    );
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddEndpointsApiExplorer();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //permite servir archivos estaticos como imagenes
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
