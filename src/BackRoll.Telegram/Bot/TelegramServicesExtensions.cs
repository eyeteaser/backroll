using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace BackRoll.Telegram.Bot
{
    public static class TelegramServicesExtensions
    {
        public static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<TelegramWorker>();
            services.Configure<TelegramBotConfig>(configuration.GetSection(TelegramBotConfig.CONFIG_SECTION));
            services.AddSingleton(p => new TelegramBotClient(p.GetRequiredService<IOptions<TelegramBotConfig>>().Value.Secret));

            return services;
        }
    }
}
