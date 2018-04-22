using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuWallpaperPlayer.Taskbar
{
    class TaskbarMenu : ContextMenu
    {
        TaskbarOption TaskbarOption { get; }

        MenuItem closeItem;

        public TaskbarMenu(TaskbarOption taskbarOption)
        {
            TaskbarOption = taskbarOption;

            (closeItem = new MenuItem
            {
                Text = "닫기",
            }).Click += OnCloseItemClick;

            MenuItems.Add(closeItem);
        }

        private void OnCloseItemClick(object sender, EventArgs e)
        {
            TaskbarOption.Application.Exit();
        }
    }
}
