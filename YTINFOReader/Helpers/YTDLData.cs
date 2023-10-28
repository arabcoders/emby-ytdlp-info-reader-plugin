using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YTINFOReader.Helpers
{
    /// <summary>
    /// Object that represent how data from yt-dlp should look like.
    /// </summary>
    public class YTDLData
    {
        // direct object id be it either channel id or video id
        public string id { get; set; }
        public string uploader { get; set; }
        public string upload_date { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string channel_id { get; set; }
        public string track { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
#nullable enable
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long? epoch { get; set; }
        public MediaBrowser.Model.IO.FileSystemMetadata? file_path { get; set; }
#nullable disable
        public List<ThumbnailInfo> thumbnails { get; set; }
    }
}
