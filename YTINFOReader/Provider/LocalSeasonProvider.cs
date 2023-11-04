using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Logging;

namespace YTINFOReader.Provider
{
    public class LocalSeasonProvider : ILocalMetadataProvider<Season>, IHasItemChangeMonitor
    {
        protected readonly ILogger _logger;
        public string Name => Constants.PLUGIN_NAME;
        public LocalSeasonProvider(ILogger logger)
        {
            _logger = logger;
            Utils.Logger = logger;
        }
        public Task<MetadataResult<Season>> GetMetadata(ItemInfo info, LibraryOptions LibraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.Info($"YIR Season GetMetadata: {info.Path}");
            MetadataResult<Season> result = new();

            var item = new Season
            {
                Name = Path.GetFileNameWithoutExtension(info.Path)
            };
            result.Item = item;
            result.HasMetadata = true;
            return Task.FromResult(result);
        }
        public bool HasChanged(BaseItem item, LibraryOptions LibraryOptions, IDirectoryService directoryService)
        {
            return true;
        }
    }
}
