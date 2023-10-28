using System.Collections.Generic;

namespace YTINFOReader.Helpers
{
    /// <summary>
    /// Object should match how YTDL json looks.
    /// </summary>
    public class ThumbnailInfo
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string resolution { get; set; }
        public string id { get; set; }
    }
}
