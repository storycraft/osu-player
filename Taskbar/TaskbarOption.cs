using System;
using System.Drawing;
using System.Windows.Forms;

namespace osu_player.Taskbar
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

            TaskbarIcon.ContextMenu = new TaskbarMenu(this);

            TaskbarIcon.Icon = SystemIcons.Application;
            TaskbarIcon.MouseClick += OnTaskbarIconClick;
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

        public String InfoMessage
        {
            get
            {
                return TaskbarIcon.Text;
            }

            set
            {
                TaskbarIcon.Text = value;
            }
        }

        private void OnTaskbarIconClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

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
