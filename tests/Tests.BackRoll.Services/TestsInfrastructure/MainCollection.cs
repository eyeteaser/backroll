using Xunit;

namespace Tests.BackRoll.Services.TestsInfrastructure
{
    [CollectionDefinition(TestsConstants.MainCollectionName)]
    public class MainCollection :
        ICollectionFixture<ConfigurationFixture>
    {
    }
}
