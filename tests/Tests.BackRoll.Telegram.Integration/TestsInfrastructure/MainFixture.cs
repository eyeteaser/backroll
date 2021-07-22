using System;
using System.Net.Http;
using BackRoll.Telegram;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.BackRoll.Telegram.Integration.TestsInfrastructure
{
    public class MainFixture
    {
        public HttpClient Client { get; }

        public IServiceProvider Services { get; }

        public MainFixture()
        {
            var factory = new WebApplicationFactory<Startup>();
            Client = factory.CreateClient();
            Services = factory.Services;
        }
    }
}
