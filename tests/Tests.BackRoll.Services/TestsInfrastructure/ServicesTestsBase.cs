using System.Linq;
using AutoMapper;
using BackRoll.Services.Models;
using Xunit.Abstractions;

namespace Tests.BackRoll.Services.TestsInfrastructure
{
    public abstract class ServicesTestsBase
    {
        protected IMapper Mapper { get; }

        private readonly ITestOutputHelper _testOutputHelper;

        protected ServicesTestsBase(
            ConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
        {
            Mapper = configurationFixture.Mapper;
            _testOutputHelper = testOutputHelper;
        }

        protected void LogTrack(Track track)
        {
            _testOutputHelper.WriteLine($"Track: {track.Name}");
            _testOutputHelper.WriteLine($"Artists: {string.Join(',', track.Artists.Select(x => x.Name))}");
            _testOutputHelper.WriteLine($"Url: {track.Url}");
        }
    }
}
