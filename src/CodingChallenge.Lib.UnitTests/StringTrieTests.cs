using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.DataStructures.Graphs;
using FluentAssertions;
using Functional.Maybe;
using NUnit.Framework;
using System.Linq;

namespace CodingChallenge.Lib.UnitTests
{
    [TestFixture]
    public class StringTrieTests
    {
        [Test]
        public void should_index_new_string_and_retrieve_it_also_type_1()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("a"));

            // Assert
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(4);
            results.Contains("abcd").Should().BeTrue();
            results.Contains("abef").Should().BeTrue();
            results.Contains("acef").Should().BeTrue();
            results.Contains("adef").Should().BeTrue();
        }

        [Test]
        public void should_index_new_string_and_retrieve_it_also_type_2()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("ab"));

            // Assert
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            results.Contains("abcd").Should().BeTrue();
            results.Contains("abef").Should().BeTrue();
        }

        [Test]
        public void should_index_new_string_and_retrieve_it_also_type_3()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("abc"));

            // Assert
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(1);
            results.Contains("abcd").Should().BeTrue();
        }

        [Test]
        public void should_return_empty_if_search_string_is_not_found()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var result = trie.Search(TrieSearchInput<string>.Create("bc"));

            // Assert
            addResult1.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeTrue();
        }

        [Test]
        public void should_support_pagination_1()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("a", 2.ToMaybe(), Maybe<string>.Nothing));

            // Assert
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            var results = result.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            result.ResultValue.ContinuationToken.Should().NotBeNull();
            results.Contains("abcd").Should().BeTrue();
            results.Contains("abef").Should().BeTrue();
        }

        [Test]
        public void should_support_pagination_with_continuation_token()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("a", 2.ToMaybe(), Maybe<string>.Nothing));
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            result.ResultValue.SearchResults.Count().Should().Be(2);
            result.ResultValue.ContinuationToken.Should().NotBeNull();

            var nextResult = trie.Search(TrieSearchInput<string>.Create("a", 3.ToMaybe(), result.ResultValue.ContinuationToken.ToMaybe()));

            // Assert
            var results = nextResult.ResultValue.SearchResults;
            results.Count().Should().Be(2);
            nextResult.ResultValue.ContinuationToken.Should().BeEmpty();
            results.Contains("acef").Should().BeTrue();
            results.Contains("adef").Should().BeTrue();
        }

        [Test]
        public void should_support_pagination_with_continuation_token_1()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var result = trie.Search(TrieSearchInput<string>.Create("a", 2.ToMaybe(), Maybe<string>.Nothing));
            addResult1.IsSuccess.Should().BeTrue();
            addResult2.IsSuccess.Should().BeTrue();
            addResult3.IsSuccess.Should().BeTrue();
            addResult4.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            result.ResultValue.SearchResults.Count().Should().Be(2);
            result.ResultValue.ContinuationToken.Should().NotBeNull();

            var nextResult = trie.Search(TrieSearchInput<string>.Create("a", 2.ToMaybe(), result.ResultValue.ContinuationToken.ToMaybe()));
            var nextResult1 = trie.Search(TrieSearchInput<string>.Create("a", 2.ToMaybe(), nextResult.ResultValue.ContinuationToken.ToMaybe()));

            // Assert
            var results = nextResult1.ResultValue.SearchResults;
            results.Count().Should().Be(0);
            nextResult1.ResultValue.ContinuationToken.Should().BeNullOrEmpty();
        }

        //[Test]
        //public void should_serialize_and_deserialize()
        //{
        //    // Arrange
        //    var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));
        //    var serializer = new InMemoryStringTrieSerializer();

        //    // Act
        //    var addResult1 = trie.AddIndex("abcd");
        //    var addResult2 = trie.AddIndex("abef");
        //    var addResult3 = trie.AddIndex("acef");
        //    var addResult4 = trie.AddIndex("adef");
        //    addResult1.IsSuccess.Should().BeTrue();
        //    addResult2.IsSuccess.Should().BeTrue();
        //    addResult3.IsSuccess.Should().BeTrue();
        //    addResult4.IsSuccess.Should().BeTrue();
        //    byte[] serializedContent = null;
        //    using(var ms = new MemoryStream())
        //    {
        //        var sresult = serializer.Serialize(trie, ms);
        //        sresult.IsSuccess.Should().BeTrue();
        //        serializedContent = ms.ToArray();
        //    }

        //    NonEmptyRootStringTrie deserializedTrie = null;
        //    using (var ms = new MemoryStream(serializedContent))
        //    {
        //        var dresult = serializer.Deserialize(ms, StringComparison.OrdinalIgnoreCase);
        //        dresult.IsSuccess.Should().BeTrue();
        //        deserializedTrie = dresult.ResultValue;
        //    }
        //    var result = deserializedTrie.Search(TrieSearchInput<string>.Create("a"));

        //    // Assert
        //    result.IsSuccess.Should().BeTrue();
        //    var results = result.ResultValue.SearchResults;
        //    results.Count().Should().Be(4);
        //    results.Contains("abcd").Should().BeTrue();
        //    results.Contains("abef").Should().BeTrue();
        //    results.Contains("acef").Should().BeTrue();
        //    results.Contains("adef").Should().BeTrue();
        //}

        [Test]
        public void should_get_approximate_size_of_trie()
        {
            // Arrange
            var trie = new NonEmptyRootStringTrie(new DirectedAcyclicGraph<UnicodeCharacter>(new UnicodeCharacter('a')));

            // Act
            var addResult1 = trie.AddIndex("abcd");
            var addResult2 = trie.AddIndex("abef");
            var addResult3 = trie.AddIndex("acef");
            var addResult4 = trie.AddIndex("adef");
            var sizeInBytes = trie.ApproximateSizeInBytes;

            // Assert
            sizeInBytes.Should().NotBe(0);
        }
    }
}
