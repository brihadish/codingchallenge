using CodingChallenge.Lib.DataStructures;
using CodingChallenge.Lib.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.UnitTests
{
    public sealed class InMemoryTrieStore<T> : ITrieDurableStore<T>
    {
        private TrieStreamStore<T> _store;
        private ConcurrentDictionary<string, MemoryStream> _storage = new ConcurrentDictionary<string, MemoryStream>();

        public InMemoryTrieStore()
        {
            _store = new TrieStreamStore<T>((name, mode) =>
            {
                switch(mode)
                {
                    case StreamMode.Write:
                        return _storage.AddOrUpdate(name, new MemoryStream(), (o, e) => new MemoryStream());

                    case StreamMode.Read:
                        MemoryStream stream;
                        if(_storage.TryGetValue(name, out stream))
                        {
                            return stream;
                        }
                        throw new InvalidOperationException();
                }
                throw new InvalidOperationException();
            });
        }

        public Task<Result<ITrie<T>>> GetTrieAsync(string trieName)
        {
            MemoryStream str;
            if(_storage.TryGetValue(trieName, out str) == false)
            {
                return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieStoreErrorType.TrieEmpty)));
            }
            return _store.GetTrieAsync(trieName);
        }

        public async Task<Result> SaveTrieAsync(string trieName, ITrie<T> trie)
        {
            return await _store.SaveTrieAsync(trieName, trie);
        }
    }
}
