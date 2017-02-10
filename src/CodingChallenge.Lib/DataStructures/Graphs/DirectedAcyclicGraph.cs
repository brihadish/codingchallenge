using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public sealed class DirectedAcyclicGraph<T> : IDirectedAcyclicGraph<T>
    {
        private readonly List<GraphVertex<T>> _graphAdjacencyList = new List<GraphVertex<T>>();

        internal IReadOnlyList<GraphVertex<T>> GraphAdjacencyList
        {
            get
            {
                return _graphAdjacencyList;
            }
        }

        public DirectedAcyclicGraph(T headNodeLabel)
        {
            if(headNodeLabel.ToMaybe().HasValue == false)
            {
                throw new ArgumentNullException(nameof(headNodeLabel));
            }
            _graphAdjacencyList.Add(new GraphVertex<T>(headNodeLabel, 0));
        }

        internal DirectedAcyclicGraph(List<GraphVertex<T>> graphAdjacencyList)
        {
            _graphAdjacencyList = graphAdjacencyList;
        }

        public void AddEdge(GraphNode<T> fromNode, GraphNode<T> toNode)
        {
            if(fromNode.NodeIndex.HasValue == false)
            {
                throw new ArgumentException(nameof(fromNode));
            }
            if(fromNode.NodeIndex.Value >= _graphAdjacencyList.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(fromNode));
            }
            var fromVertex = _graphAdjacencyList[fromNode.NodeIndex.Value];
            var toVertex = new GraphVertex<T>(toNode.NodeLabel);
            _graphAdjacencyList.Add(toVertex);
            var toVertexIndex = _graphAdjacencyList.Count - 1;
            toVertex.VertexIndex = toVertexIndex;
            fromVertex.AddAdjacentVertexIndex(toVertex.VertexLabel, toVertexIndex);
        }

        public IDirectedAcyclicGraphDepthFirstTraversor<T> GetDepthFirstEnumerator()
        {
            return new DirectedAcyclicGraphDepthFirstTraversor<T>(this);
        }
    }
}
