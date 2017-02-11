using CodingChallenge.Lib.Domain;
using CodingChallenge.Lib.Infrastructure;
using Owin;
using System;
using System.IO;
using System.Web.Http;

namespace CodingChallenge.WebApi
{
    public partial class Startup
    {
        private readonly DirectoryInfo _rootDirectory;
        private readonly FileInfo _sampleDataFile;

        private const int TrieCacheSizeInBytes = 100 * 1024 * 1024; // 100 KB
        private readonly TimeSpan TrieCacheLockAcquisitionTimeout = TimeSpan.FromMinutes(1);

        private ITrieDurableStore<string> _trieStore;
        private IZipCodeIndexer _zipCodeIndexer;
        private IStringIndexer _locationIndexer;

        public Startup(FileInfo sampleDataFile, DirectoryInfo rootDirectory)
        {
            _sampleDataFile = sampleDataFile;
            _rootDirectory = rootDirectory;
        }

        public void ConfigureApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            BuildIndexesFromSampleInput();
            ConfigureDependencyInjection(config);
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
            config.EnsureInitialized();
        }
    }
}
