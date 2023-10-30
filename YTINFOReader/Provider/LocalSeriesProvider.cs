using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using MediaBrowser.Model.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
            _logger.Debug("YTLocalSeries GetSeriesInfo: {Path}", path);
            Matcher matcher = new Matcher();
            matcher.AddInclude("**/*.info.json");
            Regex rxc = new Regex(Constants.CHANNEL_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rxp = new Regex(Constants.PLAYLIST_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (rxc.IsMatch(file) || rxp.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.Debug("YTLocalSeries GetSeriesInfo Result: {InfoPath}", infoPath);
            return infoPath;
        }

        public Task<MetadataResult<Series>> GetMetadata(ItemInfo info, LibraryOptions LibraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.Debug("YTLocalSeries GetMetadata: {Path}", info.Path);
            MetadataResult<Series> result = new MetadataResult<Series>();
            string infoPath = GetSeriesInfo(info.Path);
            if (string.IsNullOrEmpty(infoPath))
            {
                return Task.FromResult(result);
            }
            var infoJson = Utils.ReadYTDLInfo(infoPath, directoryService.GetFile(info.Path), cancellationToken);
            result = Utils.YTDLJsonToSeries(infoJson);
            _logger.Debug("YTLocalSeries GetMetadata Result: {Result}", result);
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
            _logger.Debug("YTLocalSeries HasChanged: {Path}", item.Path);
            var infoPath = GetSeriesInfo(item.Path);
            var result = false;
            if (!string.IsNullOrEmpty(infoPath))
            {
                var infoJson = GetInfoJson(infoPath);
                result = infoJson.Exists && _fileSystem.GetLastWriteTimeUtc(infoJson) < item.DateLastSaved;
            }
            _logger.Debug("YTLocalSeries HasChanged Result: {Result}", result);
            return result;

        }
    }
}
