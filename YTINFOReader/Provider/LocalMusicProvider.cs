using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider
{
    public class LocalMusicProvider : AbstractLocalProvider<LocalMusicProvider, MusicVideo>
    {
        public override string Name => Constants.PLUGIN_NAME;
        public LocalMusicProvider(IFileSystem fileSystem, ILogger logger) : base(fileSystem, logger) { }
        internal override MetadataResult<MusicVideo> GetMetadataImpl(YTDLData jsonObj)
        {
            return Utils.YTDLJsonToMusicVideo(jsonObj);
        }
    }
}
