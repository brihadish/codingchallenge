using CodingChallenge.Lib.DataStructures.Graphs;
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
            if(graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            _graph = graph;
        }

        public void AddIndex(string indexValue)
        {
            var traversor = _graph.GetDepthFirstEnumerator();
            if(traversor.MoveNext())
            {
                var firstCharInGraph = traversor.Current;
                if(firstCharInGraph.Equals(indexValue.First()) == false)
                {
                    throw new ArgumentException(nameof(indexValue));
                }
            }
            var indexValueLength = indexValue.Length;
            GraphNode<char> current;
            int index = 1;
            for(; index < indexValueLength; index++)
            {
                bool found = false;
                while(traversor.MoveNext())
                {
                    current = traversor.Current;
                    if(current.NodeLabel.Equals(indexValue[index]))
                    {
                        found = true;
                        break;
                    }
                }
                if(found == false)
                {
                    break;
                }
            }
        }
    }
}
