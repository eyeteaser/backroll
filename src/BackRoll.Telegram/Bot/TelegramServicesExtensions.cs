using BackRoll.Telegram.Configuration;
using BackRoll.Telegram.Data;
using BackRoll.Telegram.Data.Repositories;
using BackRoll.Telegram.Scenes;
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
            services.AddSingleton<ITelegramUserConfiguration, TelegramUserConfiguration>();
            services.AddSingleton<ITelegramUserConfigurationRepository, TelegramUserConfigurationRepository>();
            services.AddSingleton(p => DbContextFactory.Create(p.GetRequiredService<IOptions<TelegramBotConfig>>().Value.DbConnectionString));

            services.AddSingleton<MessageScene>();
            services.AddSingleton<SetServiceScene>();
            services.AddSingleton<IScenesManager, ScenesManager>();

            return services;
        }
    }
}
