using System;
using System.Collections.Generic;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public interface IDirectedAcyclicGraphDepthFirstTraversor<T> where T : ValueObject<T>
    {
        GraphNode<T> Current { get; }

        bool MoveNext(Func<T, bool> adjacentVertexSelector = null);

        void Reset();

        void Checkpoint();

        IEnumerable<GraphNode<T>> CurrentPath { get; }
    }
}
