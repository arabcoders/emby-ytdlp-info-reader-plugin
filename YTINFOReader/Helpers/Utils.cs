using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using MediaBrowser.Model.Logging;

namespace YTINFOReader.Helpers
{
    public class Utils
    {
#nullable enable
        public static ILogger? Logger { get; set; }
#nullable disable
        /// <summary>
        /// Regex for matching channel id.
        /// </summary>
        private static readonly Regex rxc = new(Constants.CHANNEL_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>
        /// Regex for matching playlist id.
        /// </summary>
        private static readonly Regex rxp = new(Constants.PLAYLIST_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>
        /// Regex for matching video id.
        /// </summary>
        private static readonly Regex rx = new(Constants.VIDEO_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsFresh(FileSystemMetadata fileInfo)
        {
            if (fileInfo.Exists && DateTime.UtcNow.Subtract(fileInfo.LastWriteTimeUtc.UtcDateTime).Days <= 10)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Returns the Youtube ID from the file path. Matches last 11 character field inside square brackets.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetYTID(string name)
        {
            if (rxc.IsMatch(name))
            {
                MatchCollection match = rxc.Matches(name);
                return match[0].Groups["id"].ToString();
            }

            if (rxp.IsMatch(name))
            {
                MatchCollection match = rxp.Matches(name);
                return match[0].Groups["id"].ToString();
            }

            if (rx.IsMatch(name))
            {
                MatchCollection match = rx.Matches(name);
                return match[0].Groups["id"].ToString();
            }
            return "";
        }

        public static bool IsYouTubeContent(string name)
        {
            return GetYTID(name) != "";
        }

        /// <summary>
        /// Creates a person object of type director for the provided name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="channel_id"></param>
        /// <returns></returns>
        public static PersonInfo CreatePerson(string name, string channel_id)
        {
            return new PersonInfo
            {
                Name = name,
                Type = PersonType.Director,
                ProviderIds = new ProviderIdDictionary(new Dictionary<string, string> { { Constants.PLUGIN_NAME, channel_id } }),
            };
        }

        /// <summary>
        /// Returns path to where metadata json file should be.
        /// </summary>
        /// <param name="appPaths"></param>
        /// <param name="youtubeID"></param>
        /// <returns></returns>
        public static string GetVideoInfoPath(IServerApplicationPaths appPaths, string youtubeID)
        {
            var dataPath = Path.Combine(appPaths.CachePath, Constants.PLUGIN_NAME, youtubeID);
            return Path.Combine(dataPath, "ytvideo.info.json");
        }

        /// <summary>
        /// Reads JSON data from file.
        /// </summary>
        /// <param name="metaFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static YTDLData ReadYTDLInfo(string fpath, FileSystemMetadata path, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string jsonString = File.ReadAllText(fpath);
            YTDLData data = JsonSerializer.Deserialize<YTDLData>(jsonString, new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            });
            data.file_path = path;
            return data;
        }

        /// <summary>
        /// Provides a Movie Metadata Result from a json object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static MetadataResult<Movie> YTDLJsonToMovie(YTDLData json)
        {
            var item = new Movie();
            var result = new MetadataResult<Movie>
            {
                HasMetadata = true,
                Item = item
            };
            result.Item.Name = json.title;
            result.Item.Overview = json.description;
            var date = new DateTime(1970, 1, 1);
            try
            {
                date = DateTime.ParseExact(json.upload_date, "yyyyMMdd", null);
            }
            catch
            {

            }
            result.Item.ProductionYear = date.Year;
            result.Item.PremiereDate = date;
            result.AddPerson(CreatePerson(json.uploader, json.channel_id));
            result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.id);

            return result;
        }

        /// <summary>
        /// Provides a MusicVideo Metadata Result from a json object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static MetadataResult<MusicVideo> YTDLJsonToMusicVideo(YTDLData json)
        {
            var item = new MusicVideo();
            var result = new MetadataResult<MusicVideo>
            {
                HasMetadata = true,
                Item = item
            };
            result.Item.Name = string.IsNullOrEmpty(json.track) ? json.title : json.track;
            result.Item.Artists = new List<string> { json.artist }.ToArray();
            result.Item.Album = json.album;
            result.Item.Overview = json.description;
            var date = new DateTime(1970, 1, 1);
            try
            {
                date = DateTime.ParseExact(json.upload_date, "yyyyMMdd", null);
            }
            catch { }
            result.Item.ProductionYear = date.Year;
            result.Item.PremiereDate = date;
            result.AddPerson(CreatePerson(json.uploader, json.channel_id));
            result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.id);

            return result;
        }

        /// <summary>
        /// Provides a Episode Metadata Result from a json object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static MetadataResult<Episode> YTDLJsonToEpisode(YTDLData json)
        {
            var item = new Episode();
            var result = new MetadataResult<Episode>
            {
                HasMetadata = true,
                Item = item
            };
            result.Item.Name = json.title;
            result.Item.Overview = json.description;
            var date = new DateTime(1970, 1, 1);
            try
            {
                date = DateTime.ParseExact(json.upload_date, "yyyyMMdd", null);
            }
            catch { }
            result.Item.ProductionYear = date.Year;
            result.Item.PremiereDate = date;
            result.AddPerson(CreatePerson(json.uploader, json.channel_id));
            result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd"));
            result.Item.ParentIndexNumber = int.Parse(date.ToString("yyyy"));
            result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.id);

            if (json.epoch != null)
            {
                Logger?.Debug($"Using epoch for episode index number for {json.id} {json.title}.");
                result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd") + DateTimeOffset.FromUnixTimeSeconds(json.epoch ?? new long()).ToString("mmss"));
            }

            if (json.epoch == null && json.file_path != null)
            {
                Logger?.Debug($"Using file last write time for episode index number for {json.id} {json.title}.");
                result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd") + json.file_path.LastWriteTimeUtc.ToString("mmss"));
            }

            if (json.file_path == null && json.epoch == null)
            {
                Logger?.Error($"No file or epoch data found for {json.id} {json.title}.");
            }

            return result;
        }
        /// <summary>
        /// Provides a MusicVideo Metadata Result from a json object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static MetadataResult<Series> YTDLJsonToSeries(YTDLData json)
        {
            var item = new Series();
            var result = new MetadataResult<Series>
            {
                HasMetadata = true,
                Item = item
            };

            var identifier = json.channel_id;
            var nameEx = "[" + json.id + "]";
            result.Item.Name = json.title;
            result.Item.Overview = json.description;

            if (rxc.IsMatch(nameEx))
            {
                MatchCollection match = rxc.Matches(nameEx);
                identifier = match[0].Groups["id"].ToString();
            }
            else
            {
                if (rxp.IsMatch(nameEx))
                {
                    MatchCollection match = rxp.Matches(nameEx);
                    identifier = match[0].Groups["id"].ToString();
                }
            }

            result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, identifier);
            return result;
        }
    }

}
