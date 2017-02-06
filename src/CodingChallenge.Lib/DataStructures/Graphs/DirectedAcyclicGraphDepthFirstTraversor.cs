using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public sealed partial class DirectedAcyclicGraph<T> : IDirectedAcyclicGraphDepthFirstTraversor<T>
    {
        private readonly int _currentIndex;

        public GraphNode<T> Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
