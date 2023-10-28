using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Entities.Movies;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider
{
    public class LocalMovieProvider : AbstractLocalProvider<LocalMovieProvider, Movie>
    {
        public override string Name => Constants.PLUGIN_NAME;
        public LocalMovieProvider(IFileSystem fileSystem, ILogger logger) : base(fileSystem, logger) { }
        internal override MetadataResult<Movie> GetMetadataImpl(YTDLData jsonObj)
        {
            return Utils.YTDLJsonToMovie(jsonObj);
        }
    }
}
