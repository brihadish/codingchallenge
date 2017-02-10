using CodingChallenge.Lib.DataStructures.Graphs;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures
{
    public sealed class StringTrie : ITrie<string>
    {
        private readonly IDirectedAcyclicGraph<char> _graph;

        private readonly StringComparison _comparison;

        internal DirectedAcyclicGraph<char> Graph
        {
            get
            {
                return _graph as DirectedAcyclicGraph<char>;
            }
        }

        public long ApproximateSizeInBytes
        {
            get
            {
                var vertexCharSize = 16;
                var vertexIndexSize = 32;
                return Graph.GraphAdjacencyList.Count * (vertexCharSize + vertexIndexSize);
            }
        }

        internal StringTrie(IDirectedAcyclicGraph<char> graph, StringComparison comparison)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            _graph = graph;
            _comparison = comparison;
        }

        public static StringTrie CreateNew(char startingChar, StringComparison comparison)
        {
            var graph = new DirectedAcyclicGraph<char>(startingChar);
            return new StringTrie(graph, comparison);
        }

        public Result<bool> AddIndex(string indexValue)
        {
            var traversor = _graph.GetDepthFirstEnumerator();
            if (traversor.MoveNext())
            {
                // Does the value's first character match the head vertex?
                var firstCharInGraph = traversor.Current;
                if (firstCharInGraph.NodeLabel.Equals(indexValue.First()) == false)
                {
                    return Result<bool>.Failure(Error.CreateFromEnum(TrieErrorType.IndexMismatch));
                }
            }

            var indexValueLength = indexValue.Length;
            int index = 1; // Start from second character since the first one will be same as head vertex.
            for (; index < indexValueLength; index++)
            {
                bool found = traversor.MoveNext(t => t.Equals(indexValue[index]));
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
                    _graph.AddEdge(current, GraphNode<char>.CreateNew(indexValue[index]));
                    traversor.MoveNext(t => t.Equals(indexValue[index]));
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
                if (firstCharInGraph.NodeLabel.Equals(searchValue.First()) == false)
                {
                    return Result<TrieSearchOutput<string>>.Failure(Error.CreateFromEnum(TrieErrorType.IndexMismatch));
                }
            }

            var searchResults = new List<string>();
            var searchValueLength = searchValue.Length;
            int index = 1;// Start from second character since the first one will be same as head vertex.
            for (; index < searchValueLength; index++)
            {
                bool found = traversor.MoveNext(t => t.Equals(searchValue[index]));
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
                if(searchInput.ContinuationToken.IsSomething())
                {
                    if(string.Equals(searchInput.ContinuationToken.Value, searchValue, _comparison) == false)
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
                            searchResult.Append(node.NodeLabel);
                        }
                        var searchResultValue = searchResult.ToString();
                        if(canAddToResultset)
                        {
                            searchResults.Add(searchResultValue);
                            searchResultSize++;
                        }
                        else
                        {
                            if(searchInput.ContinuationToken.Value.Equals(searchResultValue, _comparison))
                            {
                                canAddToResultset = true;
                            }
                        }
                        if(searchInput.PageSize.SelectOrElse(t => t, () => int.MaxValue) == searchResultSize)
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
