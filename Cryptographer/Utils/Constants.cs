using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptographer.Utils
{
    internal class Constants
    {
        public static float scorePrintThreshold = 10;

        public static byte maxDepth = 13;

        public static void Set(Dictionary<string, string?> args)
        {

            float.TryParse(args.GetValueOrDefault("score_print"), out scorePrintThreshold);

            byte.TryParse(args.GetValueOrDefault("maxdepth"), out maxDepth);
        }
    }
}
