namespace YTINFOReader.Helpers
{
    public class Constants
    {
        public const string PLUGIN_NAME = "YTINFOReader";
        public const string PLUGIN_GUID = "83b77e24-9fce-4ee0-a794-73fdfa972e64";
        public const string CHANNEL_URL = "https://www.youtube.com/channel/{0}";
        public const string CHANNEL_RX = @"(?<=\[)(?:youtube-)?(?<id>(UC|HC)[a-zA-Z0-9\-_]{22})(?=\])";
        public const string VIDEO_URL = "https://www.youtube.com/watch?v={0}";
        public const string VIDEO_RX = @"(?<=\[)(?:youtube-)?(?<id>[a-zA-Z0-9\-_]{11})(?=\])";

        public const string PLAYLIST_RX = @"\[(?:youtube\-)?(?<id>PL[^\[\]]{16}|PL[^\[\]]{32}|(UU|FL|LP|RD)[^\[\]]{22})\]";

        public const string PLAYLIST_URL = "https://www.youtube.com/playlist?list={0}";
    }
}
