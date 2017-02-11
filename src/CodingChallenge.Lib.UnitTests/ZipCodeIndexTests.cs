using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.Domain;
using CodingChallenge.Lib.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.UnitTests
{
    [TestFixture]
    public class ZipCodeIndexTests
    {
        [Test]
        public async Task should_add_and_retrieve_zipcode_index_1()
        {
            // Arrange
            var store = new Mock<ITrieDurableStore<string>>();
            store.Setup(t => t.GetTrieAsync(It.IsAny<string>())).Returns(
                Task.FromResult(Result<ITrie<string>>.Failure(
                    Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileNotFound))));
            var cache = new SimpleStringIndexCache(store.Object, 1024 * 1024, TimeSpan.FromMinutes(2));
            var zipCodeIndex = new ZipCodeIndexer(new StringIndexer(cache));

            // Act
            var addResult1 = await zipCodeIndex.AddZipCodeAsync(453731);
            var addResult2 = await zipCodeIndex.AddZipCodeAsync(453732);
            var addResult3 = await zipCodeIndex.AddZipCodeAsync(453733);
            var addResult4 = await zipCodeIndex.AddZipCodeAsync(453740);
            var addResult5 = await zipCodeIndex.AddZipCodeAsync(453741);
            var addResult6 = await zipCodeIndex.AddZipCodeAsync(453742);

            var result = await zipCodeIndex.SearchAsync(new ZipCodeIndexSearchInput(4, 2, null));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains(453731).Should().BeTrue();
            results.Contains(453732).Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public async Task should_add_and_retrieve_zipcode_index_2()
        {
            // Arrange
            var store = new Mock<ITrieDurableStore<string>>();
            store.Setup(t => t.GetTrieAsync(It.IsAny<string>())).Returns(
                Task.FromResult(Result<ITrie<string>>.Failure(
                    Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileNotFound))));
            var cache = new SimpleStringIndexCache(store.Object, 1024 * 1024, TimeSpan.FromMinutes(2));
            var zipCodeIndex = new ZipCodeIndexer(new StringIndexer(cache));

            // Act
            var addResult1 = await zipCodeIndex.AddZipCodeAsync(453731);
            var addResult2 = await zipCodeIndex.AddZipCodeAsync(453732);
            var addResult3 = await zipCodeIndex.AddZipCodeAsync(453733);
            var addResult4 = await zipCodeIndex.AddZipCodeAsync(453740);
            var addResult5 = await zipCodeIndex.AddZipCodeAsync(453741);
            var addResult6 = await zipCodeIndex.AddZipCodeAsync(453742);

            var result = await zipCodeIndex.SearchAsync(new ZipCodeIndexSearchInput(45374, 2, null));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains(453740).Should().BeTrue();
            results.Contains(453741).Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}
