using CodingChallenge.Lib.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public interface IStringTrieSerializer<in S> where S : Stream
    {
        Result Serialize(StringTrie trie, Stream stream);

        Result<StringTrie> Deserialize(Stream stream, StringComparison comparison);
    }
}
