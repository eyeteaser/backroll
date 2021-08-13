using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackRoll.Telegram
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Configure().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static IHostBuilder Configure(this IHostBuilder hostBuilder) =>
            hostBuilder
                .ConfigureServices((ctx, services) =>
                {
                    services.AddApplicationInsightsTelemetry(o =>
                        o.ConnectionString = ctx.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
                });
    }
}
