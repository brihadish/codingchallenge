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

        bool MoveNext();

        void Reset();
    }
}
