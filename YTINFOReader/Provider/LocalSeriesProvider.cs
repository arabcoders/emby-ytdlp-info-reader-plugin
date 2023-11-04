using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using Microsoft.Extensions.FileSystemGlobbing;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider
{
    public class LocalSeriesProvider : AbstractLocalProvider<LocalSeriesProvider, Series>
    {
        public override string Name => Constants.PLUGIN_NAME;

        public LocalSeriesProvider(IFileSystem fileSystem, ILogger logger) : base(fileSystem, logger) { }

        internal override MetadataResult<Series> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToSeries(jsonObj);

        private static string GetSeriesInfo(string path)
        {
            Matcher matcher = new();
            matcher.AddInclude("**/*.info.json");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            return infoPath;
        }

        public override Task<MetadataResult<Series>> GetMetadata(ItemInfo info, LibraryOptions LibraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.Debug($"YIR Series GetMetadata: {info.Path}");
            MetadataResult<Series> result = new();
            string infoPath = GetSeriesInfo(info.Path);
            if (string.IsNullOrEmpty(infoPath))
            {
                _logger.Debug($"YIR Series GetMetadata Result: No info.json file was found.");
                return Task.FromResult(result);
            }

            _logger.Debug($"YIR Series GetMetadata Result: {infoPath}");

            var infoJson = Utils.ReadYTDLInfo(infoPath, directoryService.GetFile(info.Path), cancellationToken);
            result = GetMetadataImpl(infoJson);
            _logger.Debug($"YIR Series GetMetadata Result: {result}");

            return Task.FromResult(result);
        }
    }
}
