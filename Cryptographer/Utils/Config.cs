namespace Cryptographer.Utils
{
    internal class Config
    {
        /// <summary> Minimum score needed for an output to be considered plaintext, 0 = Default </summary>
        public static float scorePrintThreshold = 10;

        /// <summary> Max time the search can go on for (in seconds), 0 = Default </summary>
        public static int searchTimeout = int.MaxValue;

        /// <summary> The max depth of the search, 0 = Default </summary>
        public static byte maxDepth = 24;

        /// <summary> Amount of cpu cores to be used by the program, 0 = all of them </summary>
        public static byte threadCount = (byte)Environment.ProcessorCount;

        /// <summary> Use trigrams instead of quadgrams when determining plaintext </summary>
        public static bool useTrigrams = false;

        /// <summary> Shows the ciphers used to get to the plaintext </summary>
        public static bool showStackTrace = false;

        private static void TryParse<T>(Dictionary<string, string> args, string key, ref T arg)
        {
            if (!args.TryGetValue(key, out var str)) return;

            T converted = arg;
            try
            {
                converted = (T)Convert.ChangeType(str!, typeof(T));

                if (converted.Equals(default(T)) && typeof(T) != typeof(bool))
                    converted = arg;

            }
            catch { }
            arg = converted;
        }

        public static void TrySet(Dictionary<string, string> args)
        {
            if (args.Count == 0) return;

            // this is ugly
            TryParse(args, "plaintext", ref scorePrintThreshold);
            TryParse(args, "maxdepth", ref maxDepth);
            TryParse(args, "threads", ref threadCount);
            TryParse(args, "trigrams", ref useTrigrams);
            TryParse(args, "stacktrace", ref showStackTrace);
            TryParse(args, "timeout", ref searchTimeout);
        }

        private static List<string> cfgList = new() { "plaintext", "maxdepth", "threads", "trigrams", "stacktrace", "timeout" };
        public static void SetFromFile(string path)
        {
            IniFile file = new(path);
            if (file == null) file = new("config.ini"); // fallback
            if (file == null) return;

            Dictionary<string, string> args = new();

            foreach (string name in cfgList)
            {
                string value = file.Read(name, "Cryptographer");
                args[name] = value;
            }

            TrySet(args);
        }

        public static Dictionary<string, string> CLIargs = new();
        public static void SetFromCLI(string[] args)
        {
            Dictionary<string, string> argDict = new();
            foreach (string s in args)
            {
                string[] split = s.Split("=");
                argDict.TryAdd(split[0], split[1]);
            }
            CLIargs = argDict;

            if (argDict.TryGetValue("cfg", out string? path)) // custom config file
                SetFromFile(path);

            TrySet(argDict); // override config with cli arguments
        }
    }
}
