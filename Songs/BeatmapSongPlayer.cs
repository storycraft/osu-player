using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.IO.Stores;
using OsuUtil.Beatmap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_player.Songs
{
    public class BeatmapSongPlayer
    {
        public CustomPaper Application { get; }
        public AudioManager Audio { get => Application.Audio; }

        public TrackBass CurrentTrack { get; private set; }

        private SongInfo currentSong;
        public SongInfo CurrentSong
        {
            get
            {
                return currentSong;
            }

            set
            {
                currentSong = value;

                using (FileStream fs = new FileStream(value.Song, FileMode.Open))
                {
                    Stream = new MemoryStream();

                    byte[] buffer = new byte[65535];

                    int readed;
                    while ((readed = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        Stream.Write(buffer, 0, readed);
                    }
                }

                CurrentTrack = new TrackBass(Stream);

                Audio.Track.AddItem(CurrentTrack);
            }
        }

        public Stream Stream { get; private set; }

        public double CurrentTime { get => CurrentTrack.CurrentTime; set => CurrentTrack.Seek(value); }
        public double PlayTime { get => CurrentTrack.Length; }
        public double PlayRate { get => CurrentTrack.Rate; set => CurrentTrack.Rate = value; }
        public bool IsPlaying { get => CurrentTrack.IsRunning; }
        public bool HasCompleted { get => CurrentTrack.HasCompleted; }

        public BeatmapSongPlayer(CustomPaper application)
        {
            Application = application;
        }

        public void Play()
        {
            CurrentTrack.Start();
        }

        public void Stop()
        {
            CurrentTrack.Stop();
        }
    }
}
