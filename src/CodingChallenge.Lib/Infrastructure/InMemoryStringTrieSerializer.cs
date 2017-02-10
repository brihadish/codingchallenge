using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingChallenge.Lib.DataStructures;
using System.Runtime.Serialization.Formatters.Binary;
using CodingChallenge.Lib.DataStructures.Graphs;

namespace CodingChallenge.Lib.Infrastructure
{
    public sealed class InMemoryStringTrieSerializer : IStringTrieSerializer<MemoryStream>
    {
        public Result<StringTrie> Deserialize(Stream stream, StringComparison comparison)
        {
            var ms = stream as MemoryStream;
            if(ms == null)
            {
                return Result<StringTrie>.Failure(Error.CreateFromEnum(TrieSerializerErrorType.StreamMismatch));
            }
            ms.Seek(0, SeekOrigin.Begin);
            var formatter = new BinaryFormatter();
            var graphAdjacencyList = (List<GraphVertex<char>>)formatter.Deserialize(ms);
            var trie = new StringTrie(new DirectedAcyclicGraph<char>(graphAdjacencyList), comparison);
            return Result<StringTrie>.Ok(trie);
        }

        public Result Serialize(StringTrie trie, Stream stream)
        {
            var ms = stream as MemoryStream;
            if (ms == null)
            {
                return Result.Failure(Error.CreateFromEnum(TrieSerializerErrorType.StreamMismatch));
            }
            ms.Seek(0, SeekOrigin.Begin);
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, trie.Graph.GraphAdjacencyList);
            return Result.Ok();
        }
    }
}
