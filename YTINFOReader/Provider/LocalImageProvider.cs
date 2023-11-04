using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Logging;
using Microsoft.Extensions.FileSystemGlobbing;
using MediaBrowser.Controller.Entities.TV;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider
{
    public class LocalImageProvider : ILocalImageProvider, IHasOrder
    {
        public string Name => Constants.PLUGIN_NAME;
        public int Order => 1;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        public LocalImageProvider(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            Utils.Logger = logger;
        }
        public bool Supports(BaseItem item) => item is Movie || item is Episode || item is MusicVideo;
        private string GetSeriesInfo(string path)
        {
            _logger.Debug($"YIR Image GetSeriesInfo: {path}");
            Matcher matcher = new Matcher();
            matcher.AddInclude("*.jpg");
            matcher.AddInclude("*.png");
            matcher.AddInclude("*.webp");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (Utils.RX_V.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.Debug($"YIR Image GetSeriesInfo Result: {infoPath}");
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
            _logger.Debug($"YIR Image GetImages: {item.Name}");
            var list = new List<LocalImageInfo>();
            string jpgPath = GetSeriesInfo(item.ContainingFolderPath);
            if (string.IsNullOrEmpty(jpgPath))
            {
                return list;
            }
            var localimg = new LocalImageInfo();
            var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
            localimg.FileInfo = fileInfo;
            list.Add(localimg);
            return list;
        }
    }
}
