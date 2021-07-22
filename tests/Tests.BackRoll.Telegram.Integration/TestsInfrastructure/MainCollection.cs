using Xunit;

namespace Tests.BackRoll.Telegram.Integration.TestsInfrastructure
{
    [CollectionDefinition(TestsConstants.MainCollectionName)]
    public class MainCollection : ICollectionFixture<MainFixture>
    {
    }
}
