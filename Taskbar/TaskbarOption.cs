using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuWallpaperPlayer.Taskbar
{
    public class TaskbarOption
    {
        public CustomPaper Application { get; }

        public NotifyIcon TaskbarIcon { get; }
        public SettingMenu CurrentSettingMenu { get; private set; }

        public TaskbarOption(CustomPaper application)
        {
            Application = application;

            TaskbarIcon = new NotifyIcon();

            TaskbarIcon.Icon = SystemIcons.Application;
            TaskbarIcon.Click += OnTaskbarIconClick;
        }

        public bool Visible
        {
            get
            {
                return TaskbarIcon.Visible;
            }

            set
            {
                TaskbarIcon.Visible = value;
            }
        }

        public Icon Icon
        {
            get
            {
                return TaskbarIcon.Icon;
            }

            set
            {
                TaskbarIcon.Icon = value;
            }
        }

        private void OnTaskbarIconClick(object sender, EventArgs e)
        {
            if (CurrentSettingMenu == null || CurrentSettingMenu.IsDisposed)
                CurrentSettingMenu = CreateSettingMenu();

            if (CurrentSettingMenu.Visible)
            {
                CurrentSettingMenu.Focus();
            }
            else
            {
                CurrentSettingMenu.Show();
            }
        }

        private SettingMenu CreateSettingMenu()
        {
            return new SettingMenu();
        }
    }
}
