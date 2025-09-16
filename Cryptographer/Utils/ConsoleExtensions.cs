using System.Text;

namespace Cryptographer.Utils
{
    // TODO: turn this into ConsoleExtensions using extension(Console) when c# 14 fully releases
    internal class CLIUtils
    {
        // to bypass the limit and reduce slowness
        public static string ReadLine()
        {
            StringBuilder read = new();
            StreamWriter stdout = new(Console.OpenStandardOutput());
            stdout.AutoFlush = true;

            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                char keyChar = key.KeyChar;
                if (keyChar == 0) continue; // dont stdout stuff like the windows key

                if (key.Key == ConsoleKey.Enter)
                    break;

                read.Append(keyChar);
                stdout.Write(keyChar);
            }

            return read.ToString();
        }

        public static void ClearLine()
        {
            int top = Console.CursorTop;

            Console.SetCursorPosition(0, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top);
        }
    }
}
