using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using MediaBrowser.Model.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;
using MediaBrowser.Model.Configuration;

namespace YTINFOReader.Provider
{
    public class LocalSeriesProvider : ILocalMetadataProvider<Series>, IHasItemChangeMonitor
    {
        public string Name => Constants.PLUGIN_NAME;
        protected readonly ILogger _logger;
        protected readonly IFileSystem _fileSystem;
        public LocalSeriesProvider(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            Utils.Logger = logger;
        }
        private string GetSeriesInfo(string path)
        {
            _logger.Debug($"YIR Series GetSeriesInfo: {path}");
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
            _logger.Debug($"YIR Series GetSeriesInfo Result: {infoPath}");
            return infoPath;
        }
        public Task<MetadataResult<Series>> GetMetadata(ItemInfo info, LibraryOptions LibraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.Debug($"YIR Series GetMetadata: {info.Path}");
            MetadataResult<Series> result = new();
            string infoPath = GetSeriesInfo(info.Path);
            if (string.IsNullOrEmpty(infoPath))
            {
                return Task.FromResult(result);
            }
            var infoJson = Utils.ReadYTDLInfo(infoPath, directoryService.GetFile(info.Path), cancellationToken);
            result = Utils.YTDLJsonToSeries(infoJson);
            _logger.Debug($"YIR Series GetMetadata Result: {result}");
            return Task.FromResult(result);
        }
        FileSystemMetadata GetInfoJson(string path)
        {
            var fileInfo = _fileSystem.GetFileSystemInfo(path);
            var directoryInfo = fileInfo.IsDirectory ? fileInfo : _fileSystem.GetDirectoryInfo(Path.GetDirectoryName(path));
            var directoryPath = directoryInfo.FullName;
            var specificFile = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(path) + ".info.json");
            var file = _fileSystem.GetFileInfo(specificFile);
            return file;
        }
        public bool HasChanged(BaseItem item, LibraryOptions LibraryOptions, IDirectoryService directoryService)
        {
            _logger.Debug($"YIR Series HasChanged: {item.Path}");
            var infoPath = GetSeriesInfo(item.Path);
            var result = false;
            if (!string.IsNullOrEmpty(infoPath))
            {
                var infoJson = GetInfoJson(infoPath);
                result = infoJson.Exists && infoJson.LastWriteTimeUtc.ToUniversalTime() < item.DateLastSaved.ToUniversalTime();
            }
            string status = result ? "Changed" : "Not changed";

            _logger.Debug($"YIR Series HasChanged Result: {status}");
            return result;
        }
    }
}
