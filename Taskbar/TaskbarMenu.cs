using OsuUtil.Beatmap;
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

        internal MenuItem collectionSelection;
        internal MenuItem notSelected;

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

            collectionSelection = new MenuItem
            {
                Text = "재생 목록 선택",
            };

            notSelected = new MenuItem
            {
                Text = "선택 안함",
            };
            notSelected.Click += OnCollectionReset;

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
            MenuItems.Add(collectionSelection);
            MenuItems.Add(toggleWallpaper);
            MenuItems.Add(closeItem);
        }

        internal void OnLoad()
        {
            notSelected.Checked = !TaskbarOption.Application.SongSelector.CollectionMode;
            collectionSelection.MenuItems.Add(notSelected);

            foreach (string name in TaskbarOption.Application.CollectionDb.CollectionSet.Keys)
            {
                if (TaskbarOption.Application.CollectionDb.CollectionSet[name].Count < 1)
                    continue;

                List<IBeatmap> beatmapList = new List<IBeatmap>();
                foreach (string hash in TaskbarOption.Application.CollectionDb.CollectionSet[name])
                {
                    if (!TaskbarOption.Application.OsuDb.HasBeatmapHash(hash))
                        continue;

                    IBeatmap map = TaskbarOption.Application.OsuDb.GetBeatmapByHash(hash);

                    beatmapList.Add(map);
                }

                if (beatmapList.Count < 1)
                    continue;

                MenuItem item = new MenuItem
                {
                    Text = name + " - ( " + beatmapList.Count + " )"
                };

                item.Click += (object sender, EventArgs e) =>
                {
                    OnCollectionSelect(item, beatmapList);
                };

                collectionSelection.MenuItems.Add(item);
            }
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

        private void OnCollectionReset(object sender, EventArgs e)
        {
            if (notSelected.Checked)
                return;

            foreach (MenuItem childItem in notSelected.Parent.MenuItems)
                childItem.Checked = false;

            notSelected.Checked = true;

            TaskbarOption.Application.SongSelector.CollectionMode = false;
        }

        private void OnCollectionSelect(MenuItem item, List<IBeatmap> list)
        {
            if (item.Checked)
                return;

            foreach (MenuItem childItem in item.Parent.MenuItems)
                childItem.Checked = false;

            item.Checked = true;

            TaskbarOption.Application.SongSelector.CollectionMode = true;
            TaskbarOption.Application.SongSelector.PlayCollection = list;
        }
    }
}
