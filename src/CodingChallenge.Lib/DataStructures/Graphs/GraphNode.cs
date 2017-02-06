using Functional.Maybe;
using System;
using System.Collections.Generic;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public struct GraphNode<T>
    {
        public Maybe<int> NodeIndex { get; }

        public T NodeLabel { get; }

        public Maybe<IEnumerable<GraphNode<T>>> AdjacentNodes { get; }

        private GraphNode(T nodeLabel)
        {
            var maybeT = nodeLabel.ToMaybe();
            if (maybeT.HasValue == false)
            {
                throw new ArgumentNullException(nameof(nodeLabel));
            }
            NodeIndex = Maybe<int>.Nothing;
            NodeLabel = nodeLabel;
            AdjacentNodes = Maybe<IEnumerable<GraphNode<T>>>.Nothing;
        }

        public GraphNode(int nodeIndex, T nodeLabel, IEnumerable<GraphNode<T>> adjacentNodes)
        {
            var maybeT = nodeLabel.ToMaybe();
            if (maybeT.HasValue == false)
            {
                throw new ArgumentNullException(nameof(nodeLabel));
            }
            if (nodeIndex <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeIndex));
            }
            NodeIndex = nodeIndex.ToMaybe();
            NodeLabel = nodeLabel;
            AdjacentNodes = adjacentNodes.ToMaybe();
        }

        public static GraphNode<T> CreateNew(T nodeLabel)
        {
            return new GraphNode<T>(nodeLabel);
        }
    }
}
