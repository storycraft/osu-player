using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Colour;
using OpenTK.Graphics;
using OpenTK;
using StoryWallpaper;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Threading.Tasks;
using OsuWallpaperPlayer.Taskbar;
using OsuWallpaperPlayer.Visualization;
using System.IO;
using OsuUtil;
using OsuWallpaperPlayer.Songs;
using OsuUtil.DataBase;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;

namespace OsuWallpaperPlayer
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

            Window.Visible = true;

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
                DesktopWallpaper.CurrentSongInfo = value;
                SongPlayer.CurrentSong = value;
                TaskbarOption.InfoMessage = "Now Playing: " + value.ArtistName + " - " + value.Title;

                SongPlayer.Play();
            }
        }

        private void PlayRandomSong()
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
            DesktopTool.UpdateWallpaperArea();
        }
    }
}
