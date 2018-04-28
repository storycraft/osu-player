using osu.Framework;
using osu.Framework.Allocation;
using OpenTK;
using StoryWallpaper;
using System.Drawing;
using System.Windows.Forms;
using System;
using osu_player.Taskbar;
using osu_player.Visualization;
using System.IO;
using OsuUtil;
using osu_player.Songs;
using OsuUtil.DataBase;

namespace osu_player
{
    public class CustomPaper : Game
    {
        public TaskbarOption TaskbarOption { get; }

        public BeatmapSongPlayer SongPlayer { get; private set; }
        public SongSelector SongSelector { get; private set; }

        public DesktopWallpaper DesktopWallpaper { get; private set; }

        public BeatmapDb OsuDb { get; private set; }
        public DirectoryInfo OsuFolder { get => new DirectoryInfo(OsuFinder.TryFindOsuLocation()); }
        public DirectoryInfo SongsFolder { get => new DirectoryInfo(OsuFolder.FullName + Path.DirectorySeparatorChar + "Songs"); }

        public event EventHandler OnSongChange;
        
        public CustomPaper()
        {
            Name = "CustomPaper";

            TaskbarOption = new TaskbarOption(this);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Window.IconChanged += UpdateTaskbarIcon;

            TaskbarOption.Visible = true;

            if (!OsuFinder.IsOsuInstalled())
            {
                MessageBox.Show("osu!가 설치되어 있지 않은 것 같습니다.\n프로그램을 종료합니다.", "CustomPaper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit();
            }

            OsuDb = OsuDbReader.ParseFromStream(new FileStream(OsuFolder.FullName + Path.DirectorySeparatorChar + "osu!.db", FileMode.Open));

            SongSelector = new SongSelector(OsuDb, SongsFolder);
            SongPlayer = new BeatmapSongPlayer(this);

            WallpaperMode = true;

            Host.Exited += OnExited;

            DesktopWallpaper = new DesktopWallpaper(this);

            Add(DesktopWallpaper.PlayerDrawable);

            AppendToDesktop();

            PlayRandomSong();
        }

        private void AppendToDesktop()
        {
            Rectangle virtualRect = SystemInformation.VirtualScreen;

            Window.WindowBorder = WindowBorder.Hidden;

            Window.Location = new Point(-virtualRect.Location.X, -virtualRect.Location.Y);
            Window.Size = Screen.PrimaryScreen.Bounds.Size;
            
            DesktopTool.AppendToWallpaperArea(Window.WindowInfo.Handle);
        }

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

                OnSongChange?.Invoke(this, EventArgs.Empty);

                DesktopWallpaper.CurrentSongInfo = value;
                SongPlayer.CurrentSong = value;
                TaskbarOption.InfoMessage = "Now Playing: " + value.ArtistNameUnicode + " - " + value.TitleUnicode;

                SongPlayer.Play();
            }
        }

        public void PlayRandomSong()
        {
            try
            {
                CurrentSong = SongSelector.GetRandom();

                Console.WriteLine("Now Playing : " + CurrentSong.Title);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.StackTrace);
                PlayRandomSong();
            }
        }

        private bool wallpaperMode;

        public bool WallpaperMode
        {
            get => wallpaperMode;

            set
            {
                if (wallpaperMode == value) return;

                wallpaperMode = value;

                if (wallpaperMode)
                {
                    Window.Visible = true;

                    Host.DrawThread.InactiveHz = 0;
                    Host.UpdateThread.InactiveHz = 0;
                }
                else
                {
                    Window.Visible = false;

                    Host.DrawThread.InactiveHz = 1;
                    Host.UpdateThread.InactiveHz = 10;

                    DesktopTool.UpdateWallpaper();
                }
            }
        }

        private void UpdateTaskbarIcon(object sender, EventArgs e)
        {
            TaskbarOption.Icon = Window.Icon;
        }

        protected override void Update()
        {
            base.Update();

            if (SongPlayer.HasCompleted)
                PlayRandomSong();
        }

        protected void OnExited()
        {
            TaskbarOption.Visible = false;
            DesktopTool.UpdateWallpaper();
        }
    }
}
