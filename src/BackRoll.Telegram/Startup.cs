using AutoMapper;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Services;
using BackRoll.Services.Spotify;
using BackRoll.Services.YandexMusic;
using BackRoll.Telegram.Bot;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackRoll.Telegram
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddSpotify(Configuration)
                .AddYandexMusic(Configuration)
                .AddServices()
                .AddTelegram(Configuration);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(IStreamingService).Assembly);
            });
            services.AddSingleton(x => config.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
