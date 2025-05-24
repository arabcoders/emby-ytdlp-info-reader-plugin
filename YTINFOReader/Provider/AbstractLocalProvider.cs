using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Controller.Entities.TV;

namespace YTINFOReader.Provider;

public abstract class AbstractLocalProvider<B, T> : ILocalMetadataProvider<T>, IHasItemChangeMonitor where T : BaseItem
{
    protected readonly ILogger _logger;
    protected readonly IFileSystem _fileSystem;

    public abstract string Name { get; }

    public AbstractLocalProvider(IFileSystem fileSystem, ILogger logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        Utils.Logger = logger;
    }

    protected FileSystemMetadata GetInfoJson(string path)
    {
        _logger.Debug($"{Name}.GetInfoJson: '{path}'.");
        var fileInfo = _fileSystem.GetFileSystemInfo(path);
        var directoryInfo = fileInfo.IsDirectory ? fileInfo : _fileSystem.GetDirectoryInfo(Path.GetDirectoryName(path));
        var directoryPath = directoryInfo.FullName;
        var specificFile = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(path) + ".info.json");
        var file = _fileSystem.GetFileInfo(specificFile);
        return file;
    }

    public virtual bool HasChanged(BaseItem item, LibraryOptions libraryOptions, IDirectoryService directoryService)
    {
        _logger.Debug($"{Name}.HasChanged: Checking if '{item.Path}' has updated info since last save.");

        FileSystemMetadata fileInfo;

        if (item is Series or Season)
        {
            var stringFile = Utils.GetSeriesInfo(item.Path);
            if (string.IsNullOrEmpty(stringFile))
            {
                return true;
            }
            fileInfo = directoryService.GetFile(stringFile);
        }
        else
        {
            fileInfo = directoryService.GetFile(item.Path);
        }

        if (!fileInfo.Exists)
        {
            _logger.Error($"{Name}.HasChanged: '{item.Path}' does not exist.");
            return true;
        }

        if (fileInfo.LastWriteTimeUtc.ToUniversalTime() > item.DateLastSaved.ToUniversalTime())
        {
            _logger.Info($"{Name}.HasChanged: '{item.Path}' has changed.");
            return true;
        }

        return false;
    }

    public virtual Task<MetadataResult<T>> GetMetadata(ItemInfo info, LibraryOptions libraryOptions, IDirectoryService directoryService, CancellationToken cancellationToken)
    {
        _logger.Debug($"${Name}.GetMetadata: {info.Path}");

        var result = new MetadataResult<T>();
        var metaFile = Path.ChangeExtension(info.Path, "info.json");
        if (!File.Exists(metaFile))
        {
            return Task.FromResult(result);
        }
        var jsonObj = Utils.ReadYTDLInfo(info.Path, metaFile, cancellationToken);
        _logger.Debug($"${Name}.GetMetadata: final result '{jsonObj}'.");
        result = GetMetadataImpl(jsonObj);

        return Task.FromResult(result);
    }

    internal abstract MetadataResult<T> GetMetadataImpl(YTDLData jsonObj);
}
