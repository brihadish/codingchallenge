using CodingChallenge.Lib.DataStructures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public sealed class SimpleStringIndexCache : IStringIndexCache
    {
        private readonly ITrieDurableStore<string> _trieStore;
        private readonly ConcurrentDictionary<char, ITrie<string>> _tries =
            new ConcurrentDictionary<char, ITrie<string>>();
        private readonly Queue<char> _queue = new Queue<char>();
        private readonly long _maxSizeInBytes;
        private readonly TimeSpan _lockAcquisitionTimeout;

        public SimpleStringIndexCache(ITrieDurableStore<string> trieStore, long maxSizeInBytes, TimeSpan lockAcquisitionTimeout)
        {
            if (trieStore == null)
            {
                throw new ArgumentNullException(nameof(trieStore));
            }
            _trieStore = trieStore;
            _maxSizeInBytes = maxSizeInBytes;
            _lockAcquisitionTimeout = lockAcquisitionTimeout;
        }

        public async Task<Result<ITrie<string>>> GetTrieAsync(char headNodeLabel)
        {
            if (_tries.ContainsKey(headNodeLabel))
            {
                return Result<ITrie<string>>.Ok(_tries[headNodeLabel]);
            }
            var trieResult = await _trieStore.GetTrieAsync(headNodeLabel.ToString());
            if (trieResult.IsSuccess)
            {
                var trie = trieResult.ResultValue;
                var result = await AddToCache(headNodeLabel, trie);
                if (result.IsFailure)
                {
                    // Can't do anything here other than logging the error.
                }
                return Result<ITrie<string>>.Ok(trie);
            }
            else
            {
                TrieFileStoreErrorType errorType;
                if (Enum.TryParse(trieResult.Error.ErrorType, out errorType))
                {
                    switch (errorType)
                    {
                        case TrieFileStoreErrorType.TrieFileNotFound:
                        case TrieFileStoreErrorType.TrieFileEmpty:
                            return Result<ITrie<string>>.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.TrieNotFound));

                        case TrieFileStoreErrorType.InsufficientPermissionError:
                        case TrieFileStoreErrorType.TrieFileReadError:
                            return Result<ITrie<string>>.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.UnableToParseTrie));
                    }
                }
                return Result<ITrie<string>>.Failure(Error.Create($"TrieFileStoreErrorType.{trieResult.Error.ErrorType}"));
            }
        }

        public Task<Result> AddTrieAsync(char headNodeLabel, ITrie<string> trie)
        {
            return AddToCache(headNodeLabel, trie);
        }

        private async Task<Result> AddToCache(char headNodeLabel, ITrie<string> trie)
        {
            long currentSize = 0;
            while ((currentSize = _tries.Values.Sum(t => t.ApproximateSizeInBytes)) >= _maxSizeInBytes)
            {
                var nodeLabelToEvict = _queue.Dequeue();
                ITrie<string> removedTrie;
                _tries.TryRemove(nodeLabelToEvict, out removedTrie);
                var result = await _trieStore.SaveTrieAsync(nodeLabelToEvict.ToString(), removedTrie);
                if (result.IsFailure)
                {
                    return Result.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.UnableToAddToCache));
                }
            }
            _tries.AddOrUpdate(
                headNodeLabel,
                c => new ThreadSafeStringTrie(trie, _lockAcquisitionTimeout),
                (o, e) => new ThreadSafeStringTrie(trie, _lockAcquisitionTimeout));
            _queue.Enqueue(headNodeLabel);
            return Result.Ok();
        }
    }
}
