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

        public static float scoreBreakSearchThreshold = 100;

        public static void Set(Dictionary<string, string?> args)
        {
            if (float.TryParse(args.GetValueOrDefault("score_print"), out float score_print))
                scorePrintThreshold = score_print;

            if (float.TryParse(args.GetValueOrDefault("score_breaksearch"), out float score_breaksearch))
                scoreBreakSearchThreshold = score_breaksearch;
        }
    }
}
