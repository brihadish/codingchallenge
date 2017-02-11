using CodingChallenge.Lib.DataStructures.Graphs;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingChallenge.Lib.DataStructures
{
    [Serializable]
    internal sealed class NonEmptyRootStringTrie : ITrie<string>
    {
        private readonly IDirectedAcyclicGraph<UnicodeCharacter> _graph;
        private readonly bool _isCaseSensitive;

        public long ApproximateSizeInBytes
        {
            get
            {
                var vertexCharSize = 16;
                var vertexIndexSize = 32;
                return _graph.VertexCount * (vertexCharSize + vertexIndexSize);
            }
        }

        public NonEmptyRootStringTrie(IDirectedAcyclicGraph<UnicodeCharacter> graph, bool isCaseSensitive)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            _graph = graph;
            _isCaseSensitive = isCaseSensitive;
        }

        public static Result<NonEmptyRootStringTrie> CreateNew(string firstIndexValue, bool isCaseSensitive)
        {
            var firstChar = firstIndexValue.FirstCharacter(isCaseSensitive);
            if (firstChar.IsNothing())
            {
                return Result<NonEmptyRootStringTrie>.Failure(Error.CreateFromEnum(TrieErrorType.EmptyIndexValue));
            }

            var graph = new DirectedAcyclicGraph<UnicodeCharacter>(firstChar.Value);
            var trie = new NonEmptyRootStringTrie(graph, isCaseSensitive);
            return Result<NonEmptyRootStringTrie>.Ok(trie);
        }

        public Result<bool> AddIndex(string indexValue)
        {
            if (string.IsNullOrWhiteSpace(indexValue))
            {
                return Result<bool>.Failure(Error.CreateFromEnum(TrieErrorType.EmptyIndexValue));
            }
            var traversor = _graph.GetDepthFirstEnumerator();
            if (traversor.MoveNext())
            {
                // Does the value's first character match the head vertex?
                var firstCharInGraph = traversor.Current;
                var firstCharInIndexValue = new UnicodeCharacter(indexValue.FirstOrDefault(), _isCaseSensitive);
                if (firstCharInGraph.NodeLabel.Equals(firstCharInIndexValue) == false)
                {
                    return Result<bool>.Failure(Error.CreateFromEnum(TrieErrorType.IndexMismatch));
                }
            }

            var indexValueLength = indexValue.Length;
            int index = 1; // Start from second character since the first one will be same as head vertex.
            for (; index < indexValueLength; index++)
            {
                var indexValueChar = new UnicodeCharacter(indexValue[index], _isCaseSensitive);
                bool found = traversor.MoveNext(t => t.Equals(indexValueChar));
                if (found == false)
                {
                    break;
                }
            }
            if (index < indexValueLength)
            {
                // The string is not present in the trie. Add the missing parts of the string to the trie.
                for (; index < indexValueLength; index++)
                {
                    var current = traversor.Current;
                    var indexValueChar = new UnicodeCharacter(indexValue[index], _isCaseSensitive);
                    _graph.AddEdge(current, GraphNode<UnicodeCharacter>.CreateNew(indexValueChar));
                    traversor.MoveNext(t => t.Equals(indexValueChar));
                }
                // Index has been added.
                return Result<bool>.Ok(true);
            }
            // Index was already present.
            return Result<bool>.Ok(false);
        }

        public Result<TrieSearchOutput<string>> Search(TrieSearchInput<string> searchInput)
        {
            if (searchInput.SearchValue.IsNothing())
            {
                return Result<TrieSearchOutput<string>>.Failure(Error.CreateFromEnum(TrieErrorType.EmptySearchValue));
            }
            var searchValue = searchInput.SearchValue.Value;
            var traversor = _graph.GetDepthFirstEnumerator();
            if (traversor.MoveNext())
            {
                // Does the value's first character match the head vertex?
                var firstCharInGraph = traversor.Current;
                var firstCharInSearchValue = new UnicodeCharacter(searchValue.FirstOrDefault(), _isCaseSensitive);
                if (firstCharInGraph.NodeLabel.Equals(firstCharInSearchValue) == false)
                {
                    return Result<TrieSearchOutput<string>>.Failure(Error.CreateFromEnum(TrieErrorType.IndexMismatch));
                }
            }

            var searchResults = new List<string>();
            var searchValueLength = searchValue.Length;
            int index = 1;// Start from second character since the first one will be same as head vertex.
            for (; index < searchValueLength; index++)
            {
                var searchValueChar = new UnicodeCharacter(searchValue[index], _isCaseSensitive);
                bool found = traversor.MoveNext(t => t.Equals(searchValueChar));
                if (found == false)
                {
                    break;
                }
            }
            var continuationToken = string.Empty;
            if (index != searchValueLength)
            {
                return Result<TrieSearchOutput<string>>.Ok(TrieSearchOutput<string>.Empty());
            }
            else if (traversor.Current.IsLeaf.SelectOrElse(t => t, () => false))
            {
                if (searchInput.ContinuationToken.IsSomething())
                {
                    var comparison = _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    if (string.Equals(searchInput.ContinuationToken.Value, searchValue, comparison) == false)
                    {
                        // Reached end of traversal, hence return.
                        searchResults.Add(searchValue);
                    }
                }
            }
            else
            {
                var searchResult = new StringBuilder();
                var searchResultSize = 0;
                var canAddToResultset = searchInput.ContinuationToken.IsNothing();
                traversor.Checkpoint();

                while (traversor.MoveNext())
                {
                    var current = traversor.Current;
                    if (current.IsLeaf.SelectOrElse(t => t, () => false))
                    {
                        foreach (var node in traversor.CurrentPath)
                        {
                            searchResult.Append(node.NodeLabel.Value);
                        }
                        var searchResultValue = searchResult.ToString();
                        if (canAddToResultset)
                        {
                            searchResults.Add(searchResultValue);
                            searchResultSize++;
                        }
                        else
                        {
                            var comparison = _isCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                            if (searchInput.ContinuationToken.Value.Equals(searchResultValue, comparison))
                            {
                                canAddToResultset = true;
                            }
                        }
                        if (searchInput.PageSize.SelectOrElse(t => t, () => int.MaxValue) == searchResultSize)
                        {
                            continuationToken = searchResultValue;
                            break;
                        }
                        searchResult = new StringBuilder();
                    }
                }
            }
            return Result<TrieSearchOutput<string>>.Ok(new TrieSearchOutput<string>(searchResults, continuationToken));
        }
    }
}
