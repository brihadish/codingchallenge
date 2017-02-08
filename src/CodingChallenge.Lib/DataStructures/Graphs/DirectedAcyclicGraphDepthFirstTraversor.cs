using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public sealed class DirectedAcyclicGraphDepthFirstTraversor<T> : IDirectedAcyclicGraphDepthFirstTraversor<T>
    {
        private int _currentIndex = -1;
        private Stack<int> _previousIndexes = new Stack<int>();
        private GraphVertex<T> _current;
        private List<int> _traversedIndexes = new List<int>();

        private readonly DirectedAcyclicGraph<T> _graph;

        public DirectedAcyclicGraphDepthFirstTraversor(DirectedAcyclicGraph<T> graph)
        {
            if(graph.GraphAdjacencyList.Count == 0)
            {
                throw new ArgumentNullException(nameof(graph));
            }
            _graph = graph;
        }

        public GraphNode<T> Current
        {
            get
            {
                return new GraphNode<T>(
                                _currentIndex, 
                                _current.VertexLabel, 
                                _current.AdjacentVertices.Count == 0 ? true : false);
            }
        }

        public bool MoveNext(Func<T, bool> adjacentVertexSelector = null)
        {
            var found = false;
            if (_currentIndex == -1)
            {
                _currentIndex++;
                found = true;
            }
            else
            {
                DetermineNextVertex();
                if(_current.AdjacentVertices.Count == 0)
                {
                    return false;
                }

                foreach(var vertex in _current.AdjacentVertices)
                {
                    if (adjacentVertexSelector != null)
                    {
                        if(adjacentVertexSelector(vertex.Item1) == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (_traversedIndexes.Contains(vertex.Item2))
                        {
                            continue;
                        }
                    }
                    _previousIndexes.Push(_currentIndex);
                    _currentIndex = vertex.Item2;
                    found = true;
                    break;
                }
                _traversedIndexes.Add(_currentIndex);
            }

            if(found)
            {
                _current = _graph.GraphAdjacencyList[_currentIndex];
            }
            return found;
        }

        public void Reset()
        {
            _currentIndex = -1;
            _previousIndexes.Clear();
        }

        public void Checkpoint()
        {
            _previousIndexes.Clear();
        }

        private void DetermineNextVertex()
        {
            if (_current.AdjacentVertices.Count > 0)
            {
                return;
            }
            while (_previousIndexes.Count > 0)
            {
                var index = _previousIndexes.Pop();
                _current = _graph.GraphAdjacencyList[index];
                foreach (var vertex in _current.AdjacentVertices)
                {
                    if (_traversedIndexes.Contains(vertex.Item2) == false)
                    {
                        break;
                    }
                }
            }
        }
    }
}
