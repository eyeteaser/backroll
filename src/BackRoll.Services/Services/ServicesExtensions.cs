using BackRoll.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackRoll.Services.Services
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IStreamingManager, StreamingManager>();

            return services;
        }
    }
}
