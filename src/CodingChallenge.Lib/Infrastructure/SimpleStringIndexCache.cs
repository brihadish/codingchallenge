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
        private readonly ConcurrentDictionary<UnicodeCharacter, ThreadSafeStringTrie> _tries =
            new ConcurrentDictionary<UnicodeCharacter, ThreadSafeStringTrie>();
        private readonly Queue<char> _queue = new Queue<char>();
        private readonly long _maxSizeInBytes;
        private readonly TimeSpan _lockAcquisitionTimeout;
        private readonly bool _isCaseSensitive;

        public SimpleStringIndexCache(
            ITrieDurableStore<string> trieStore, long maxSizeInBytes,
            TimeSpan lockAcquisitionTimeout, bool isCaseSensitive)
        {
            if (trieStore == null)
            {
                throw new ArgumentNullException(nameof(trieStore));
            }
            _trieStore = trieStore;
            _maxSizeInBytes = maxSizeInBytes;
            _lockAcquisitionTimeout = lockAcquisitionTimeout;
            _isCaseSensitive = isCaseSensitive;
        }

        public async Task<Result<ITrie<string>>> GetTrieAsync(char headNodeLabel)
        {
            var headNodeLabelChar = new UnicodeCharacter(headNodeLabel, _isCaseSensitive);
            if (_tries.ContainsKey(headNodeLabelChar))
            {
                return Result<ITrie<string>>.Ok(_tries[headNodeLabelChar]);
            }
            var trieResult = await _trieStore.GetTrieAsync(headNodeLabel.ToString());
            if (trieResult.IsSuccess)
            {
                var trie = trieResult.ResultValue;
                var result = await AddToCache(headNodeLabelChar, trie);
                if (result.IsFailure)
                {
                    // Can't do anything here other than logging the error.
                }
                return Result<ITrie<string>>.Ok(trie);
            }
            else
            {
                TrieStoreErrorType errorType;
                if (Enum.TryParse(trieResult.Error.ErrorType, out errorType))
                {
                    switch (errorType)
                    {
                        case TrieStoreErrorType.TrieNotFound:
                        case TrieStoreErrorType.TrieEmpty:
                            return Result<ITrie<string>>.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.TrieNotFound));

                        case TrieStoreErrorType.TrieDeserializationError:
                            return Result<ITrie<string>>.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.UnableToDeserializeTrie));

                        case TrieStoreErrorType.InsufficientPermissionError:
                            return Result<ITrie<string>>.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.UnableToAccessTrie));
                    }
                }
                return Result<ITrie<string>>.Failure(Error.Create($"TrieFileStoreErrorType.{trieResult.Error.ErrorType}"));
            }
        }

        public Task<Result> AddTrieAsync(char headNodeLabel, ITrie<string> trie)
        {
            var headNodeLabelChar = new UnicodeCharacter(headNodeLabel, _isCaseSensitive);
            return AddToCache(headNodeLabelChar, trie);
        }

        private async Task<Result> AddToCache(UnicodeCharacter headNodeLabelChar, ITrie<string> trie)
        {
            long currentSize = 0;
            while ((currentSize = _tries.Values.Sum(t => t.ApproximateSizeInBytes)) >= _maxSizeInBytes)
            {
                var nodeLabelToEvict = new UnicodeCharacter(_queue.Dequeue(), _isCaseSensitive);
                ThreadSafeStringTrie removedTrie;
                _tries.TryRemove(nodeLabelToEvict, out removedTrie);
                var result = await _trieStore.SaveTrieAsync(nodeLabelToEvict.ToString(), removedTrie._trie);
                if (result.IsFailure)
                {
                    return Result.Failure(Error.CreateFromEnum(SimpleStringIndexCacheErrorType.UnableToAddToCache));
                }
            }
            _tries.AddOrUpdate(
                headNodeLabelChar,
                c => new ThreadSafeStringTrie(trie, _lockAcquisitionTimeout),
                (o, e) => new ThreadSafeStringTrie(trie, _lockAcquisitionTimeout));
            _queue.Enqueue(headNodeLabelChar.Value);
            return Result.Ok();
        }
    }
}
