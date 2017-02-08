using Functional.Maybe;
using System;
using System.Collections.Generic;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public struct GraphNode<T>
    {
        public Maybe<int> NodeIndex { get; }

        public T NodeLabel { get; }        

        public Maybe<bool> IsLeaf { get; }

        private GraphNode(T nodeLabel)
        {
            var maybeT = nodeLabel.ToMaybe();
            if (maybeT.HasValue == false)
            {
                throw new ArgumentNullException(nameof(nodeLabel));
            }
            NodeIndex = Maybe<int>.Nothing;
            NodeLabel = nodeLabel;
            IsLeaf = Maybe<bool>.Nothing;
        }

        public GraphNode(int nodeIndex, T nodeLabel, bool isLeaf)
        {
            var maybeT = nodeLabel.ToMaybe();
            if (maybeT.HasValue == false)
            {
                throw new ArgumentNullException(nameof(nodeLabel));
            }
            if (nodeIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nodeIndex));
            }
            NodeIndex = nodeIndex.ToMaybe();
            NodeLabel = nodeLabel;
            IsLeaf = isLeaf.ToMaybe();
        }

        public static GraphNode<T> CreateNew(T nodeLabel)
        {
            return new GraphNode<T>(nodeLabel);
        }
    }
}
