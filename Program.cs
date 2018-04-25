using osu.Framework;
using osu.Framework.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_player
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            using (Game game = new CustomPaper())
            {
                using (GameHost host = Host.GetSuitableHost(@"custom-paper"))
                {
                    host.Run(game);
                }
            }
        }
    }
}
