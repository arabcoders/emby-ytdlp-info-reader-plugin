using MediaBrowser.Model.Plugins;
using System;

public enum IDTypes
{
    YTDLP = 1,
}

namespace YTINFOReader.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public IDTypes IDType { get; set; }
        public PluginConfiguration()
        {
            // defaults
            IDType = IDTypes.YTDLP;
        }
    }
}
