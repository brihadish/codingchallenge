using CodingChallenge.Lib.DataStructures;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public sealed class TrieFileStore<T> : ITrieDurableStore<T>
    {
        private readonly DirectoryInfo _rootDirectory;
        private readonly TrieStreamStore<T> _store;

        public TrieFileStore(DirectoryInfo rootDirectory)
        {
            _rootDirectory = rootDirectory;
            _store = new TrieStreamStore<T>((name, mode) =>
            {
                var filePath = Path.Combine(_rootDirectory.FullName, name);
                switch (mode)
                {
                    case StreamMode.Read:
                        return File.OpenRead(filePath);

                    case StreamMode.Write:
                        return File.Create(filePath);
                }
                throw new InvalidOperationException($"Unknown StreamMode:: [{mode}]");
            });
        }

        public Task<Result<ITrie<T>>> GetTrieAsync(string trieName)
        {
            var filePath = Path.Combine(_rootDirectory.FullName, trieName);
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length == 0)
                {
                    return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieStoreErrorType.TrieEmpty)));
                }
                try
                {
                    return _store.GetTrieAsync(trieName);
                }
                catch (SerializationException)
                {
                    return Task.FromResult(Result<ITrie<T>>.Failure(
                        Error.CreateFromEnum(TrieStoreErrorType.TrieDeserializationError)));
                }
                catch (SecurityException)
                {
                    return Task.FromResult(Result<ITrie<T>>.Failure(
                        Error.CreateFromEnum(TrieStoreErrorType.InsufficientPermissionError)));
                }
            }
            return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieStoreErrorType.TrieNotFound)));
        }

        public async Task<Result> SaveTrieAsync(string trieName, ITrie<T> trie)
        {
            try
            {
                return await _store.SaveTrieAsync(trieName, trie);
            }
            catch (SerializationException)
            {
                return Result.Failure(Error.CreateFromEnum(TrieStoreErrorType.TrieSerializationError));
            }
            catch (SecurityException)
            {
                return Result.Failure(Error.CreateFromEnum(TrieStoreErrorType.InsufficientPermissionError));
            }
        }
    }
}
