using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Entities.TV;
using YTINFOReader.Helpers;
using System.IO;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.FileSystemGlobbing;

namespace YTINFOReader.Provider
{
    public class YTINFOReaderImageProvider : ILocalImageProvider, IHasOrder
    {
        public string Name => Constants.PLUGIN_NAME;
        public int Order => 1;
        private readonly ILogger _logger;
        public YTINFOReaderImageProvider(ILogger logger)
        {
            _logger = logger;
            Utils.Logger = logger;
        }
        public bool Supports(BaseItem item) => item is Movie || item is Episode || item is MusicVideo || item is Series;

        /// <summary>
        /// Retrieves Image.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="directoryService"></param>
        /// <returns></returns>
        public IEnumerable<LocalImageInfo> GetImages(BaseItem item, IDirectoryService directoryService)
        {
            var list = new List<LocalImageInfo>();

            _logger.Debug($"YIR Image GetImages: {item.Name}");

            var imageFile = "";
            var extensions = new[] { ".jpg", ".png", ".webp" };

            if (item is Series)
            {
                Matcher matcher = new();
                foreach (var extension in extensions)
                {
                    matcher.AddInclude($"**/*{extension}");
                }
                foreach (string file in matcher.GetResultsInFullPath(item.Path))
                {
                    if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
                    {
                        imageFile = file;
                        break;
                    }
                }
            }
            else
            {
                foreach (var extension in extensions)
                {
                    var path = Path.ChangeExtension(item.Path, extension);
                    if (directoryService.GetFile(path).Exists)
                    {
                        imageFile = path;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(imageFile))
            {
                return list;
            }

            list.Add(new LocalImageInfo
            {
                FileInfo = directoryService.GetFile(imageFile),
                Type = ImageType.Primary
            });

            return list;
        }
    }
}
