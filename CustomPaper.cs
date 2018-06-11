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
using osu.Framework.Graphics;
using System.Collections.Generic;
using OsuUtil.Beatmap;

namespace osu_player
{
    public class CustomPaper : Game
    {
        public TaskbarOption TaskbarOption { get; }

        public BeatmapSongPlayer SongPlayer { get; private set; }
        public SongSelector SongSelector { get; private set; }

        public DesktopWallpaper DesktopWallpaper { get; private set; }

        public BeatmapDb OsuDb { get; private set; }
        public CollectionDb CollectionDb { get; private set; }

        public DirectoryInfo OsuFolder { get => new DirectoryInfo(OsuFinder.TryFindOsuLocation()); }
        public DirectoryInfo SongsFolder { get => new DirectoryInfo(OsuFolder.FullName + Path.DirectorySeparatorChar + "Songs"); }

        public event EventHandler OnSongChange;
        
        public CustomPaper()
        {
            Name = "CustomPaper";

            wallpaperMode = true;
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

            try
            {
                OsuDb = OsuDbReader.ParseFromStream(new FileStream(OsuFolder.FullName + Path.DirectorySeparatorChar + "osu!.db", FileMode.Open));
            } catch (Exception e)
            {
                MessageBox.Show("osu! 비트맵 리스트를 파싱할 수 없습니다. 프로그램이 종료됩니다.\n" + e.StackTrace, "CustomPaper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit();
            }

            try
            {
                CollectionDb = OsuCollectionReader.ParseFromStream(new FileStream(OsuFolder.FullName + Path.DirectorySeparatorChar + "collection.db", FileMode.Open));
            } catch (Exception e)
            {
                CollectionDb = new OsuCollectionDb(new Dictionary<string, List<string>>());
                MessageBox.Show("osu! 컬렉션 리스트를 파싱할 수 없습니다. 컬렉션 모드가 비 활성화 됩니다.\n" + e.StackTrace, "CustomPaper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SongSelector = new SongSelector(OsuDb, CollectionDb, SongsFolder);
            SongPlayer = new BeatmapSongPlayer(this);

            Host.Exited += OnExited;

            DesktopWallpaper = new DesktopWallpaper(this);

            Window.Visible = false;

            Add(DesktopWallpaper.PlayerDrawable);

            AppendToDesktop();

            TaskbarOption.OnLoad();

            PlayRandomSong();
        }

        private void AppendToDesktop()
        {
            Rectangle virtualRect = SystemInformation.VirtualScreen;

            Window.WindowState = WindowState.Minimized;

            DesktopTool.AppendToWallpaperArea(Window.WindowInfo.Handle);

            Window.WindowBorder = WindowBorder.Hidden;

            Window.Location = new Point(-virtualRect.Left, -virtualRect.Top);
            Window.Size = Screen.PrimaryScreen.Bounds.Size;

            DesktopTool.UpdateWallpaper();

            Window.Visible = true;
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
