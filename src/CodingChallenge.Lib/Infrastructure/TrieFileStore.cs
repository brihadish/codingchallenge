using CodingChallenge.Lib.DataStructures;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Threading.Tasks;

namespace CodingChallenge.Lib.Infrastructure
{
    public sealed class TrieFileStore<T> : ITrieDurableStore<T>
    {
        private readonly DirectoryInfo _rootDirectory;

        public TrieFileStore(DirectoryInfo rootDirectory)
        {
            _rootDirectory = rootDirectory;
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
                            Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileEmpty)));
                }
                ITrie<T> trie;
                var formatter = new BinaryFormatter();
                using (var stream = File.OpenRead(filePath))
                {
                    try
                    {
                        trie = formatter.Deserialize(stream) as ITrie<T>;
                    }
                    catch (SerializationException)
                    {
                        return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileReadError)));
                    }
                    catch (SecurityException)
                    {
                        return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieFileStoreErrorType.InsufficientPermissionError)));
                    }
                }
                return Task.FromResult(Result<ITrie<T>>.Ok(trie));
            }
            return Task.FromResult(Result<ITrie<T>>.Failure(
                            Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileNotFound)));
        }

        public Task<Result> SaveTrieAsync(string trieName, ITrie<T> trie)
        {
            var filePath = Path.Combine(_rootDirectory.FullName, trieName);
            var formatter = new BinaryFormatter();
            using (var stream = File.Create(filePath))
            {
                try
                {
                    formatter.Serialize(stream, trie);
                }
                catch (SerializationException)
                {
                    return Task.FromResult(Result.Failure(
                        Error.CreateFromEnum(TrieFileStoreErrorType.TrieFileWriteError)));
                }
                catch (SecurityException)
                {
                    return Task.FromResult(Result.Failure(
                        Error.CreateFromEnum(TrieFileStoreErrorType.InsufficientPermissionError)));
                }
            }
            return Task.FromResult(Result.Ok());
        }
    }
}
