using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Logging;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
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
        }
        public bool Supports(BaseItem item) => item is Movie || item is Episode || item is MusicVideo;

        private string GetSeriesInfo(string path)
        {
            _logger.Debug("YTLocalImage GetSeriesInfo: {Path}", path);
            Matcher matcher = new Matcher();
            Regex rx = new Regex(Constants.VIDEO_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            matcher.AddInclude("*.jpg");
            matcher.AddInclude("*.png");
            matcher.AddInclude("*.webp");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (rx.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.Debug("YTLocalImage GetSeriesInfo Result: {InfoPath}", infoPath);
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
            _logger.Debug("YTLocalImage GetImages: {Name}", item.Name);
            var list = new List<LocalImageInfo>();
            string jpgPath = GetSeriesInfo(item.ContainingFolderPath);
            if (String.IsNullOrEmpty(jpgPath))
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
