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
    public class ZipCodeIndexTests
    {
        [Test]
        public void should_add_and_retrieve_zipcode_index_1()
        {
            // Arrange
            var zipCodeIndex = new ZipCodeIndex(StringTrie.CreateNew('4', StringComparison.Ordinal));

            // Act
            var addResult1 = zipCodeIndex.AddZipCode(453731);
            var addResult2 = zipCodeIndex.AddZipCode(453732);
            var addResult3 = zipCodeIndex.AddZipCode(453733);
            var addResult4 = zipCodeIndex.AddZipCode(453740);
            var addResult5 = zipCodeIndex.AddZipCode(453741);
            var addResult6 = zipCodeIndex.AddZipCode(453742);

            var result = zipCodeIndex.Search(new ZipCodeIndexSearchInput(4, 2, null));

            // Assert
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains(453731).Should().BeTrue();
            results.Contains(453732).Should().BeTrue();
            result.ResultValue.ContinuationToken.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void should_add_and_retrieve_zipcode_index_2()
        {
            // Arrange
            var zipCodeIndex = new ZipCodeIndex(StringTrie.CreateNew('4', StringComparison.Ordinal));

            // Act
            var addResult1 = zipCodeIndex.AddZipCode(453731);
            var addResult2 = zipCodeIndex.AddZipCode(453732);
            var addResult3 = zipCodeIndex.AddZipCode(453733);
            var addResult4 = zipCodeIndex.AddZipCode(453740);
            var addResult5 = zipCodeIndex.AddZipCode(453741);
            var addResult6 = zipCodeIndex.AddZipCode(453742);

            var result = zipCodeIndex.Search(new ZipCodeIndexSearchInput(45374, 2, null));

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
