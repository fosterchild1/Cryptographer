namespace Cryptographer.Utils
{
    internal class Constants
    {
        public static float scorePrintThreshold = 10;

        public static byte maxDepth = 24;

        public static byte threadCount = (byte)Environment.ProcessorCount;

        private static void TryParse<T>(Dictionary<string, string?> args, string key, ref T arg)
        {
            if (!args.TryGetValue(key, out var str)) return;

            T converted = arg;
            try
            {
                converted = (T)Convert.ChangeType(str!, typeof(T));
            }
            catch { }

            arg = converted;
        }

        public static void Set(Dictionary<string, string?> args)
        {

            TryParse(args, "score_print", ref scorePrintThreshold);
            TryParse(args, "maxdepth", ref maxDepth);
            TryParse(args, "threads", ref threadCount);
        }
    }
}
