using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public interface IDirectedAcyclicGraphDepthFirstTraversor<T>
    {
        GraphNode<T> Current { get; }

        bool MoveNext(Func<T, bool> adjacentVertexSelector = null);

        void Reset();

        void Checkpoint();

        IEnumerable<GraphNode<T>> CurrentPath { get; }
    }
}
