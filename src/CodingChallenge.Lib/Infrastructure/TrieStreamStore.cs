using System;
using System.Threading.Tasks;
using CodingChallenge.Lib.DataStructures;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CodingChallenge.Lib.Infrastructure
{
    internal enum StreamMode
    {
        Read,
        Write
    }

    internal sealed class TrieStreamStore<T> : ITrieDurableStore<T>
    {
        private readonly Func<string, StreamMode, Stream> _streamProvider;

        public TrieStreamStore(Func<string, StreamMode, Stream> streamProvider)
        {
            if (streamProvider == null)
            {
                throw new ArgumentNullException(nameof(streamProvider));
            }
            _streamProvider = streamProvider;
        }

        public Task<Result<ITrie<T>>> GetTrieAsync(string trieName)
        {
            ITrie<T> trie;
            var formatter = new BinaryFormatter();
            using (var stream = _streamProvider(trieName, StreamMode.Read))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                trie = formatter.Deserialize(stream) as ITrie<T>;
            }
            return Task.FromResult(Result<ITrie<T>>.Ok(trie));
        }

        public Task<Result> SaveTrieAsync(string trieName, ITrie<T> trie)
        {
            var formatter = new BinaryFormatter();
            using (var stream = _streamProvider(trieName, StreamMode.Write))
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                formatter.Serialize(stream, trie);
            }
            return Task.FromResult(Result.Ok());
        }
    }
}
