using OsuUtil.Beatmap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuWallpaperPlayer.Songs
{
    public struct SongInfo
    {
        public String FolderName;

        public String Background;
        public String Song;

        public String ArtistName;
        public String ArtistNameUnicode;

        public String Title;
        public String TitleUnicode;

        public static SongInfo FromBeatmap(IBeatmap map)
        {
            return new SongInfo
            {
                Title = map.RankedName,
                TitleUnicode = map.RankedNameUnicode,
                ArtistName = map.Artist,
                ArtistNameUnicode = map.ArtistUnicode,
                FolderName = map.SongFolderName,
                Song = map.AudioFileName
            };
        }
    }
}
