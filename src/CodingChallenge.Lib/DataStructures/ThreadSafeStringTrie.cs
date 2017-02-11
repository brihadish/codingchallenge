using System;
using System.Threading;

namespace CodingChallenge.Lib.DataStructures
{
    internal sealed class ThreadSafeStringTrie : ITrie<string>
    {
        private readonly ITrie<string> _trie;
        private readonly ReaderWriterLockSlim _trieLock = 
            new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly TimeSpan _lockAcquisitionTimeout;

        public ThreadSafeStringTrie(ITrie<string> trie, TimeSpan lockAcquisitionTimeout)
        {
            _trie = trie;
            _lockAcquisitionTimeout = lockAcquisitionTimeout;
        }

        public long ApproximateSizeInBytes
        {
            get
            {
                return _trie.ApproximateSizeInBytes;
            }
        }

        public Result<bool> AddIndex(string indexValue)
        {
            _trieLock.TryEnterWriteLock(_lockAcquisitionTimeout);
            var result = _trie.AddIndex(indexValue);
            _trieLock.EnterWriteLock();
            return result;
        }

        public Result<TrieSearchOutput<string>> Search(TrieSearchInput<string> searchInput)
        {
            _trieLock.TryEnterReadLock(_lockAcquisitionTimeout);
            var result = _trie.Search(searchInput);
            _trieLock.ExitReadLock();
            return result;
        }
    }
}
