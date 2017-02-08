using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.DataStructures.Graphs;
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
    public class StringTrieTests
    {
        [Test]
        public void should_index_new_string_and_retrieve_it_also_type_1()
        {
            // Arrange
            var trie = new StringTrie(new DirectedAcyclicGraph<char>('a'));

            // Act
            trie.AddIndex("abcd");
            trie.AddIndex("abef");
            trie.AddIndex("acef");
            trie.AddIndex("adef");
            var results = trie.Search("a");

            // Assert
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
            var trie = new StringTrie(new DirectedAcyclicGraph<char>('a'));

            // Act
            trie.AddIndex("abcd");
            trie.AddIndex("abef");
            trie.AddIndex("acef");
            trie.AddIndex("adef");
            var results = trie.Search("ab");

            // Assert
            results.Count().Should().Be(2);
            results.Contains("abcd").Should().BeTrue();
            results.Contains("abef").Should().BeTrue();
        }

        [Test]
        public void should_index_new_string_and_retrieve_it_also_type_3()
        {
            // Arrange
            var trie = new StringTrie(new DirectedAcyclicGraph<char>('a'));

            // Act
            trie.AddIndex("abcd");
            trie.AddIndex("abef");
            trie.AddIndex("acef");
            trie.AddIndex("adef");
            var results = trie.Search("abc");

            // Assert
            results.Count().Should().Be(1);
            results.Contains("abcd").Should().BeTrue();
        }

        [Test]
        public void should_return_empty_if_search_string_is_not_found()
        {
            // Arrange
            var trie = new StringTrie(new DirectedAcyclicGraph<char>('a'));

            // Act
            trie.AddIndex("abcd");
            var results = trie.Search("bc");

            // Assert
            results.Count().Should().Be(0);
        }
    }
}
