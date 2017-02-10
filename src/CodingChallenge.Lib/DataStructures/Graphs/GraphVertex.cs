using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    [Serializable]
    internal sealed class GraphVertex<T>
    {
        public int VertexIndex { get; set; }

        public T VertexLabel { get; private set; }

        private readonly List<Tuple<T, int>> _adjacentVertices;

        public IList<Tuple<T, int>> AdjacentVertices
        {
            get { return _adjacentVertices; }
        }

        public GraphVertex(T vertexLabel)
        {
            VertexLabel = vertexLabel;
            _adjacentVertices = new List<Tuple<T, int>>();
        }

        public GraphVertex(T vertexLabel, int vertexIndex)
        {
            VertexLabel = vertexLabel;
            VertexIndex = vertexIndex;
            _adjacentVertices = new List<Tuple<T, int>>();
        }

        public bool AddAdjacentVertexIndex(T adjacentVertexLabel, int adjacentVertexIndex)
        {
            if(_adjacentVertices.Any(t => t.Item1.Equals(adjacentVertexLabel)) == false)
            {
                _adjacentVertices.Add(new Tuple<T, int>(adjacentVertexLabel, adjacentVertexIndex));
                return true;
            }
            return false;
        } 
    }
}
