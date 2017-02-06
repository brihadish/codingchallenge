using Functional.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public sealed partial class DirectedAcyclicGraph<T> : IDirectedAcyclicGraph<T>
    {
        private readonly List<GraphVertex<T>> _graphAdjacencyList = new List<GraphVertex<T>>();

        public DirectedAcyclicGraph(T headNodeLabel)
        {
            if(headNodeLabel.ToMaybe().HasValue == false)
            {
                throw new ArgumentNullException(nameof(headNodeLabel));
            }
            _graphAdjacencyList.Add(new GraphVertex<T>(headNodeLabel));
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
            _graphAdjacencyList.Add(new GraphVertex<T>(toNode.NodeLabel));
            var toNodeIndex = _graphAdjacencyList.Count - 1;
            fromVertex.AddAdjacentVertexIndex(toNode.NodeLabel, toNodeIndex);
        }

        public IDirectedAcyclicGraphDepthFirstTraversor<T> GetDepthFirstEnumerator()
        {
            return this;
        }
    }
}
