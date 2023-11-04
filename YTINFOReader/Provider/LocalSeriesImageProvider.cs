using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.FileSystemGlobbing;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider
{
    public class LocalSeriesImageProvider : ILocalImageProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        public string Name => Constants.PLUGIN_NAME;
        public LocalSeriesImageProvider(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            Utils.Logger = logger;
        }
        public bool Supports(BaseItem item) => item is Series;
        private string GetSeriesInfo(string path)
        {
            _logger.Debug($"YIR Image Series GetSeriesInfo: {path}");
            Matcher matcher = new();
            matcher.AddInclude("**/*.jpg");
            matcher.AddInclude("**/*.png");
            matcher.AddInclude("**/*.webp");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.Debug($"YIR Image Series GetSeriesInfo Result: {infoPath}");
            return infoPath;
        }
        /// <summary>
        /// Retrieves Image.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="directoryService"></param>
        /// <returns></returns>
        public IEnumerable<LocalImageInfo> GetImages(BaseItem item, IDirectoryService directoryService)
        {
            _logger.Debug($"YIR Image Series GetImages: {item.Name}");
            var list = new List<LocalImageInfo>();
            string jpgPath = GetSeriesInfo(item.Path);
            if (string.IsNullOrEmpty(jpgPath))
            {
                return list;
            }
            var localimg = new LocalImageInfo();
            var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
            localimg.FileInfo = fileInfo;
            list.Add(localimg);
            _logger.Debug($"YIR Image Series GetImages Result: {list}");
            return list;
        }
    }
}
