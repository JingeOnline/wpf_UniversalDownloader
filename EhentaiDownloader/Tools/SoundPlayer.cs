using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Tools
{
    public class SoundPlayer
    {
        public static bool IsOn { get; set; } = true;

        public static void PlayCompleteness()
        {
            if (IsOn)
            {
                Console.Beep(500, 100);
                Console.Beep(700, 100);
                Console.Beep(1000, 100);
            }
            else
            {
                return;
            }

        }
    }
}
