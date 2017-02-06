namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public interface IDirectedAcyclicGraph<T>
    {
        void AddEdge(GraphNode<T> fromNode, GraphNode<T> toNode);

        IDirectedAcyclicGraphDepthFirstTraversor<T> GetDepthFirstEnumerator();
    }
}
