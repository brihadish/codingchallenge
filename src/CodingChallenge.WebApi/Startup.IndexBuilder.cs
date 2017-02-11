using CodingChallenge.Lib.Domain;
using CodingChallenge.Lib.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CodingChallenge.WebApi
{
    public partial class Startup
    {
        public void BuildIndexesFromSampleInput()
        {
            Console.WriteLine("Started building index from input file {0}", _sampleDataFile.FullName);
            var watch = Stopwatch.StartNew();
            _trieStore = new TrieFileStore<string>(_rootDirectory);
            _zipCodeIndexer = new ZipCodeIndexer(
                        new StringIndexer(new SimpleStringIndexCache(
                            _trieStore, TrieCacheSizeInBytes, TrieCacheLockAcquisitionTimeout, false), false));
            _locationIndexer = new StringIndexer(new SimpleStringIndexCache(
                            _trieStore, TrieCacheSizeInBytes, TrieCacheLockAcquisitionTimeout, false), false);
            using (var reader = new StreamReader(_sampleDataFile.FullName))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine();
                    var split = line.Split('\t');
                    int zipCode = -1;
                    var location = string.Empty;

                    if (int.TryParse(split.FirstOrDefault(), out zipCode))
                    {
                        _zipCodeIndexer.AddZipCodeAsync(zipCode).GetAwaiter().GetResult();
                    }
                    if (split.Length > 1)
                    {
                        _locationIndexer.AddIndexAsync(split[1]).GetAwaiter().GetResult();
                    }
                }
            }
            watch.Stop();
            Console.WriteLine("Completed building index. Time taken = {0} seconds", watch.Elapsed.TotalSeconds);
        }
    }
}
