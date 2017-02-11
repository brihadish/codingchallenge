namespace CodingChallenge.Lib.DataStructures.Graphs
{
    public interface IDirectedAcyclicGraph<T> where T : ValueObject<T>
    {
        long VertexCount { get; }

        void AddEdge(GraphNode<T> fromNode, GraphNode<T> toNode);

        IDirectedAcyclicGraphDepthFirstTraversor<T> GetDepthFirstEnumerator();
    }
}