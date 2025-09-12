namespace Cryptographer.Utils
{
    internal class Config
    {
        public static float scorePrintThreshold = 10;

        public static byte maxDepth = 24;

        public static byte threadCount = (byte)Environment.ProcessorCount;

        public static bool useTrigrams = false;

        private static void TryParse<T>(Dictionary<string, string> args, string key, ref T arg)
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

        public static void TrySet(Dictionary<string, string> args)
        {
            if (args.Count == 0) return;

            TryParse(args, "plaintext", ref scorePrintThreshold);
            TryParse(args, "maxdepth", ref maxDepth);
            TryParse(args, "threads", ref threadCount);
            TryParse(args, "trigrams", ref useTrigrams);

            if (threadCount == 0) threadCount = (byte)Environment.ProcessorCount;
        }

        private static List<string> cfgList = new() { "plaintext", "maxdepth", "threads", "trigrams" };
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
