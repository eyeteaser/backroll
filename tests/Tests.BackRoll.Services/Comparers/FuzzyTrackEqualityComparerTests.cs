using BackRoll.Services.Comparers;
using BackRoll.Services.Models;
using FluentAssertions;
using Xunit;

namespace Tests.BackRoll.Services.Comparers
{
    public class FuzzyTrackEqualityComparerTests
    {
        private readonly FuzzyTrackEqualityComparer _sut;

        public FuzzyTrackEqualityComparerTests()
        {
            _sut = new FuzzyTrackEqualityComparer();
        }

        [Theory]
        [InlineData("Ядрёность – образ жизни", "Ядрёность - образ жизни")]
        [InlineData("Ядрёность - образ жизни", "Ядрёность – образ жизни")]
        [InlineData("Ядрёность – образ жизни", "Ядрёность - Образ Жизни (Акустическая Версия)")]
        [InlineData("Ядрёность - Образ Жизни (Акустическая Версия)", "Ядрёность – образ жизни")]
        public void Equals_SimilarTracks_ShouldMatch(string source, string target)
        {
            // arrange
            var trackA = new Track()
            {
                Name = source,
            };

            var trackB = new Track()
            {
                Name = target,
            };

            // act
            var result = _sut.Equals(trackA, trackB);

            // assert
            result.Should().BeTrue();
        }
    }
}
