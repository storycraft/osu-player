using System;
using System.Drawing;
using System.Windows.Forms;

namespace osu_player.Taskbar
{
    public class TaskbarOption
    {
        public CustomPaper Application { get; }

        public NotifyIcon TaskbarIcon { get; }
        public TaskbarMenu TaskbarMenu { get; }

        public TaskbarOption(CustomPaper application)
        {
            Application = application;

            TaskbarIcon = new NotifyIcon();

            TaskbarIcon.ContextMenu = TaskbarMenu = new TaskbarMenu(this);

            TaskbarIcon.Icon = SystemIcons.Application;
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
                TaskbarMenu.songInfoItem.Text = value;
            }
        }
    }
}
