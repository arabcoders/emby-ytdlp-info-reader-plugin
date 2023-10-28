using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;
using MediaBrowser.Model.Configuration;

namespace YTINFOReader.Provider
{
    public abstract class AbstractLocalProvider<B, T> : ILocalMetadataProvider<T>, IHasItemChangeMonitor where T : BaseItem
    {
        protected readonly ILogger _logger;
        protected readonly IFileSystem _fileSystem;

        /// <summary>
        /// Providers name, this appears in the library metadata settings.
        /// </summary>
        public abstract string Name { get; }

        public AbstractLocalProvider(IFileSystem fileSystem, ILogger logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }

        protected FileSystemMetadata GetInfoJson(string path)
        {
            _logger.Debug($"YTLocal GetInfoJson: {path}");
            var fileInfo = _fileSystem.GetFileSystemInfo(path);
            var directoryInfo = fileInfo.IsDirectory ? fileInfo : _fileSystem.GetDirectoryInfo(Path.GetDirectoryName(path));
            var directoryPath = directoryInfo.FullName;
            var specificFile = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(path) + ".info.json");
            var file = _fileSystem.GetFileInfo(specificFile);
            return file;
        }

        /// <summary>
        /// Returns bolean if item has changed since last recorded.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="directoryService"></param>
        /// <returns></returns>
        public bool HasChanged(BaseItem item, LibraryOptions LibraryOptions, IDirectoryService directoryService)
        {
            _logger.Debug("YTLocal HasChanged: {Name}", item.Name);
            var infoJson = GetInfoJson(item.Path);
            var result = infoJson.Exists && _fileSystem.GetLastWriteTimeUtc(infoJson) < item.DateLastSaved;
            _logger.Debug("YTLocal HasChanged Result: {Result}", result);
            return result;
        }

        /// <summary>
        /// Retrieves metadata of item.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="directoryService"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MetadataResult<T>> GetMetadata(ItemInfo info, LibraryOptions LibraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.Debug("YTLocal GetMetadata: {Path}", info.Path);
            var result = new MetadataResult<T>();
            var infoFile = Path.ChangeExtension(info.Path, "info.json");
            if (!File.Exists(infoFile))
            {
                return Task.FromResult(result);
            }
            var jsonObj = Utils.ReadYTDLInfo(infoFile, directoryService.GetFile(info.Path), cancellationToken);
            _logger.Debug("YTLocal GetMetadata Result: {JSON}", jsonObj.ToString());
            result = GetMetadataImpl(jsonObj);

            return Task.FromResult(result);
        }

        internal abstract MetadataResult<T> GetMetadataImpl(YTDLData jsonObj);
    }
}
