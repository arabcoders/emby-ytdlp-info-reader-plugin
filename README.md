# YouTube INFO reader plugin

This project based on Ankenyr [jellyfin-youtube-metadata-plugin](https://github.com/ankenyr/jellyfin-youtube-metadata-plugin), I removed the remote support
and added what we think make sense for episodes numbers, if you follow active channels like we do, you will notice that
some episodes will have problems in sorting or numbering, we fixed some issues that relates to what we need.

Episodes are named `1` + `MMddhhmm`, for example if the episode date is `2022-06-01 05:33:44` the episode Index number should be
`106010533` this should sort active channels match better. we wanted to add seconds to the Index number, but sadly due to the limitation
of int32 we are unable to do so. And for seasons, it should reflect the year and in this example it would be `2022`.

The reason we prefix the episode numbers by `1` is because we use two month digit, thus if you have episodes that aired `2023-07-02 00:00:00` and `2023-10-02 00:00:00`,
 it will prevent the `10-02` episode from appearing before the `07-02` episodes. Usually `0` is not counted in the index so if we do not add `1` before the
 index number the episodes index will be `70020000` vs `10020000`.

 The hours and minutes are pulled from the `epoch` field in the JSON file, if the `.info.json` file is very old and does not have the variable,
 the plugin fallback on file last modification time.

## Overview
Plugin for [Emby](https://emby.media/) that retrieves metadata for content from yt-dlp `.info.json` files.

### Features
- Reads the `.info.json` files provided by [yt-dlp](https://github.com/yt-dlp/yt-dlp) or similar programs to extract metadata from.
- Supports thumbnails of `png`, `jpg` or `webp` format for both channel and videos.
- Supports the following library types `Movies`, `Music Videos` and `Shows`.
- Supports ExternalID providing quick links to source of metadata.

## Usage

### File Naming Requirements
All media needs to have the ID embedded in the file name within square brackets.
The following are valid examples of a channel and video. We support the following id format
`[(id)]` and `[youtube-(id)]`.

### Channels.
To add metadata from channel, INFO and image should follow the following format. YouTube channels must start with UC or HC.

- `whatever title you want [(youtube-)?(UC|HC)uAXFkgsw1L7xaCfnd5JJOw].info.json`
- `whatever title you want [(youtube-)?(UC|HC)uAXFkgsw1L7xaCfnd5JJOw].(jpg|png|webp)`

### Video files and related metadata.
For Video files it follow the same rules as the channel format.

- `whatever [(youtube-)?dQw4w9WgXcQ].info.json`
- `whatever [(youtube-)?dQw4w9WgXcQ].(jpg|png|webp)`
- `whatever [(youtube-)?dQw4w9WgXcQ].mkv`

## Build and Installing from source

1. Clone or download this repository.
2. Ensure you have .NET Core SDK setup and installed.
3. Build plugin with following command.
    ```
    dotnet publish --configuration Release --output bin
    ```
4. Copy `YTINFOReader.dll` from the `bin` directory to emby plugins directory.
5. Restart emby
6. If performed correctly you will see a plugin named YTINFOReader in `Dashboard -> Plugins`.
