using BMAPI.v1;
using BMAPI.v1.Events;
using OsuUtil.Beatmap;
using OsuUtil.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuWallpaperPlayer.Songs
{
    public class SongSelector
    {
        public BeatmapDb BeatmapDb { get; }
        public DirectoryInfo SongsFolder { get; }

        public SongSelector(BeatmapDb beatmapDb, DirectoryInfo songsFolder)
        {
            BeatmapDb = beatmapDb;
            SongsFolder = songsFolder;
        }

        public SongInfo GetRandom()
        {
            List<IBeatmapSet> beatmapSetList = BeatmapDb.BeatmapSets.Values.ToList();
            IBeatmapSet mapSet = beatmapSetList[(int) (new Random().NextDouble() * (beatmapSetList.Count - 1))];

            List<IBeatmap> beatmapList = mapSet.Beatmaps.Values.ToList();
            IBeatmap map = beatmapList[(int) (new Random().NextDouble() * (mapSet.Beatmaps.Count - 1))];

            DirectoryInfo beatmapFolder = new DirectoryInfo(SongsFolder.FullName + Path.DirectorySeparatorChar + map.SongFolderName);

            Beatmap bm = new Beatmap(beatmapFolder.FullName + Path.DirectorySeparatorChar + map.OsuFileName);

            ContentEvent backgroundEvent = null;
            foreach (EventBase e in bm.Events)
            {
                if (e is ContentEvent contentEvent)
                {
                    if (contentEvent.Type == ContentType.Image)
                    {
                        backgroundEvent = contentEvent;
                        break;
                    }
                }
            }

            return new SongInfo
            {
                Title = bm.Title,
                TitleUnicode = bm.TitleUnicode,
                
                ArtistName = bm.Artist,
                ArtistNameUnicode = bm.ArtistUnicode,

                Song = beatmapFolder.FullName + Path.DirectorySeparatorChar + bm.AudioFilename,
                Background = backgroundEvent == null ? null : beatmapFolder.FullName + Path.DirectorySeparatorChar + backgroundEvent.Filename
            };
        }
    }
}
