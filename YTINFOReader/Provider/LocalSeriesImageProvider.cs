using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using System.Text.RegularExpressions;
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
        }
        public bool Supports(BaseItem item) => item is Series;

        private string GetSeriesInfo(string path)
        {
            _logger.Debug("YTLocalImageSeries GetSeriesInfo: {Path}", path);
            Matcher matcher = new Matcher();
            matcher.AddInclude("**/*.jpg");
            matcher.AddInclude("**/*.png");
            matcher.AddInclude("**/*.webp");
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
            _logger.Debug("YTLocalImageSeries GetSeriesInfo Result: {InfoPath}", infoPath);
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
            _logger.Debug("YTLocalImageSeries GetImages: {Name}", item.Name);
            var list = new List<LocalImageInfo>();
            string jpgPath = GetSeriesInfo(item.Path);
            if (String.IsNullOrEmpty(jpgPath))
            {
                return list;
            }
            var localimg = new LocalImageInfo();
            var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
            localimg.FileInfo = fileInfo;
            list.Add(localimg);
            _logger.Debug("YTLocalImageSeries GetImages Result: {Result}", list.ToString());
            return list;
        }

    }
}
