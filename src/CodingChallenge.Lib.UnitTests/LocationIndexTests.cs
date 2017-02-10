using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.Domain;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.UnitTests
{
    [TestFixture]
    public class LocationIndexTests
    {
        [Test]
        public void should_add_and_retrieve_location_index_1()
        {
            // Arrange
            var locationIndex = new LocationIndex(StringTrie.CreateNew('V', StringComparison.Ordinal));

            // Act
            var addResult1 = locationIndex.AddLocation("Vändra");
            var addResult2 = locationIndex.AddLocation("Vyukyula");
            var addResult3 = locationIndex.AddLocation("Võtikvere");
            var addResult4 = locationIndex.AddLocation("Võsupere");
            var addResult5 = locationIndex.AddLocation("Võsu");
            var addResult6 = locationIndex.AddLocation("Võsivere");

            var result = locationIndex.Search(new LocationIndexSearchInput("Võ", 2, null));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains("Võtikvere").Should().BeTrue();
            results.Contains("Võsupere").Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void should_add_and_retrieve_location_index_2()
        {
            // Arrange
            var locationIndex = new LocationIndex(StringTrie.CreateNew('V', StringComparison.Ordinal));

            // Act
            var addResult1 = locationIndex.AddLocation("Vändra");
            var addResult2 = locationIndex.AddLocation("Vyukyula");
            var addResult3 = locationIndex.AddLocation("Võtikvere");
            var addResult4 = locationIndex.AddLocation("Võsupere");
            var addResult5 = locationIndex.AddLocation("Võsu");
            var addResult6 = locationIndex.AddLocation("Võsivere");

            var result = locationIndex.Search(new LocationIndexSearchInput("V", 2, null));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains("Vändra").Should().BeTrue();
            results.Contains("Vyukyula").Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}
