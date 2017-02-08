using CodingChallenge.Lib.DataStructures.Graphs;
using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures
{
    public sealed class StringTrie
    {
        private readonly IDirectedAcyclicGraph<char> _graph;

        internal StringTrie(IDirectedAcyclicGraph<char> graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            _graph = graph;
        }

        public void AddIndex(string indexValue)
        {
            var traversor = _graph.GetDepthFirstEnumerator();
            if (traversor.MoveNext())
            {
                var firstCharInGraph = traversor.Current;
                if (firstCharInGraph.NodeLabel.Equals(indexValue.First()) == false)
                {
                    throw new ArgumentException(nameof(indexValue));
                }
            }
            var indexValueLength = indexValue.Length;
            int index = 1;
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
            }
        }

        public IEnumerable<string> Search(string searchValue)
        {
            var traversor = _graph.GetDepthFirstEnumerator();
            if (traversor.MoveNext())
            {
                var firstCharInGraph = traversor.Current;
                if (firstCharInGraph.NodeLabel.Equals(searchValue.First()) == false)
                {
                    return Enumerable.Empty<string>();
                }
            }

            var searchValueLength = searchValue.Length;
            int index = 1;
            for (; index < searchValueLength; index++)
            {
                bool found = traversor.MoveNext(t => t.Equals(searchValue[index]));
                if (found == false)
                {
                    break;
                }
            }
            if (index != searchValueLength)
            {
                return Enumerable.Empty<string>();
            }

            traversor.Checkpoint();
            var searchResults = new List<string>();
            var searchResult = new StringBuilder();
            searchResult.Append(searchValue);
            while (traversor.MoveNext())
            {
                var current = traversor.Current;
                searchResult.Append(current.NodeLabel);
                if (current.IsLeaf.SelectOrElse(t => t, () => false))
                {
                    searchResults.Add(searchResult.ToString());
                    searchResult = new StringBuilder();
                    searchResult.Append(searchValue);
                }
            }

            return searchResults;
        }
    }
}
