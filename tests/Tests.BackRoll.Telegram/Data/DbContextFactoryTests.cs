using System.IO;
using BackRoll.Telegram.Database;
using FluentAssertions;
using Xunit;

namespace Tests.BackRoll.Telegram.Data
{
    public class DbContextFactoryTests
    {
        [Fact]
        public void Create_CorrectData_ShouldCreateDirectoryIfNotExists()
        {
            // arrange
            string directory = "./data";
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            string path = $"{directory}/test.db";

            // act
            var db = DbContextFactory.Create(path);

            // assert
            db.Should().NotBeNull();
            Directory.Exists(directory).Should().BeTrue();
            File.Exists(path);
        }
    }
}
