using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_player.Taskbar
{
    public class TaskbarMenu : ContextMenu
    {
        public TaskbarOption TaskbarOption { get; }

        internal MenuItem infoItem;
        internal MenuItem songInfoItem;
        internal MenuItem nextSongItem;
        internal MenuItem toggleWallpaper;
        internal MenuItem closeItem;

        public TaskbarMenu(TaskbarOption taskbarOption)
        {
            TaskbarOption = taskbarOption;
            
            infoItem = new MenuItem
            {
                Text = "osu!player by storycraft",
                Enabled = false
            };

            songInfoItem = new MenuItem
            {
                Text = "",
                Enabled = false
            };

            (nextSongItem = new MenuItem
            {
                Text = "곡 변경",
            }).Click += OnNextSongItemClick;

            (toggleWallpaper = new MenuItem
            {
                Text = "월페이퍼 렌더링 토글",
            }).Click += OnToggleWallpaper;
            toggleWallpaper.Checked = taskbarOption.Application.WallpaperMode;

            (closeItem = new MenuItem
            {
                Text = "닫기",
            }).Click += OnCloseItemClick;

            MenuItems.Add(infoItem);
            MenuItems.Add(new MenuItem
            {
                Text = "-",
                Enabled = false
            });
            MenuItems.Add(songInfoItem);
            MenuItems.Add(nextSongItem);
            MenuItems.Add(toggleWallpaper);
            MenuItems.Add(closeItem);
        }

        private void OnToggleWallpaper(object sender, EventArgs e)
        {
            toggleWallpaper.Checked = TaskbarOption.Application.WallpaperMode = !toggleWallpaper.Checked;
        }

        private void OnNextSongItemClick(object sender, EventArgs e)
        {
            TaskbarOption.Application.PlayRandomSong();
        }

        private void OnCloseItemClick(object sender, EventArgs e)
        {
            TaskbarOption.Application.Exit();
        }
    }
}
