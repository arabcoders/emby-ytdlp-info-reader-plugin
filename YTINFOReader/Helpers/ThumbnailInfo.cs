namespace YTINFOReader.Helpers
{
    /// <summary>
    /// Object should match how YTDL json looks.
    /// </summary>
    public class ThumbnailInfo
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Resolution { get; set; }
        public string Id { get; set; }
    }
}
