using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.Domain;
using CodingChallenge.Lib.Infrastructure;
using FluentAssertions;
using Functional.Maybe;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.UnitTests
{
    [TestFixture]
    public class LocationIndexTests
    {
        [Test]
        public async Task should_add_and_retrieve_location_index_1()
        {
            // Arrange
            var store = new Mock<ITrieDurableStore<string>>();
            store.Setup(t => t.GetTrieAsync(It.IsAny<string>())).Returns(
                Task.FromResult(Result<ITrie<string>>.Failure(
                    Error.CreateFromEnum(TrieStoreErrorType.TrieNotFound))));
            var cache = new SimpleStringIndexCache(store.Object, 1024 * 1024, TimeSpan.FromMinutes(2), true);
            var locationIndex = new StringIndexer(cache, true);

            // Act
            var addResult1 = await locationIndex.AddIndexAsync("Vändra");
            var addResult2 = await locationIndex.AddIndexAsync("Vyukyula");
            var addResult3 = await locationIndex.AddIndexAsync("Võtikvere");
            var addResult4 = await locationIndex.AddIndexAsync("Võsupere");
            var addResult5 = await locationIndex.AddIndexAsync("Võsu");
            var addResult6 = await locationIndex.AddIndexAsync("Võsivere");

            var result = await locationIndex.SearchAsync(new StringIndexerSearchInput("Võ", 2.ToMaybe(), Maybe<string>.Nothing));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains("Võtikvere").Should().BeTrue();
            results.Contains("Võsupere").Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task should_add_and_retrieve_location_index_2()
        {
            // Arrange
            var store = new Mock<ITrieDurableStore<string>>();
            store.Setup(t => t.GetTrieAsync(It.IsAny<string>())).Returns(
                Task.FromResult(Result<ITrie<string>>.Failure(
                    Error.CreateFromEnum(TrieStoreErrorType.TrieNotFound))));
            var cache = new SimpleStringIndexCache(store.Object, 1024 * 1024, TimeSpan.FromMinutes(1), false);
            var locationIndex = new StringIndexer(cache, false);

            // Act
            var addResult1 = await locationIndex.AddIndexAsync("Vändra");
            var addResult2 = await locationIndex.AddIndexAsync("Vyukyula");
            var addResult3 = await locationIndex.AddIndexAsync("Võtikvere");
            var addResult4 = await locationIndex.AddIndexAsync("Võsupere");
            var addResult5 = await locationIndex.AddIndexAsync("Võsu");
            var addResult6 = await locationIndex.AddIndexAsync("Võsivere");

            var result = await locationIndex.SearchAsync(new StringIndexerSearchInput("v", 2.ToMaybe(), Maybe<string>.Nothing));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains("Vändra").Should().BeTrue();
            results.Contains("Vyukyula").Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task should_add_and_retrieve_location_index_3()
        {
            // Arrange
            var store = new InMemoryTrieStore<string>();
            var cache = new SimpleStringIndexCache(store, 1, TimeSpan.FromMinutes(2), false);
            var locationIndex = new StringIndexer(cache, false);

            // Act
            var addResult1 = await locationIndex.AddIndexAsync("Vändra");
            var addResult2 = await locationIndex.AddIndexAsync("Vyukyula");
            var addResult3 = await locationIndex.AddIndexAsync("Võtikvere");
            var addResult4 = await locationIndex.AddIndexAsync("Võsupere");
            var addResult5 = await locationIndex.AddIndexAsync("Võsu");
            var addResult6 = await locationIndex.AddIndexAsync("Võsivere");

            var result = await locationIndex.SearchAsync(new StringIndexerSearchInput("v", 2.ToMaybe(), Maybe<string>.Nothing));

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
