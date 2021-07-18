using System;
using System.Net;
using YandexMusicResolver;
using YandexMusicResolver.Config;

namespace BackRoll.Services.YandexMusic
{
    public static class YandexMusicClientFactory
    {
        public static IYandexMusicMainResolver CreateYandexMusicClient(YandexMusicConfig config)
        {
            var yandexConfig = new EmptyYandexConfig();
            if (config.UseProxy)
            {
                yandexConfig.YandexProxy = new WebProxy(new Uri($"http://{config.ProxyHost}:{config.ProxyPort}"))
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        userName: config.ProxyUserName,
                        password: config.ProxyPassword),
                };
            }

            return new YandexMusicMainResolver(yandexConfig);
        }
    }
}
