namespace Cryptographer.Utils
{
    internal class CLIUtils
    {
        public static void ClearLine()
        {
            int top = Console.CursorTop;

            Console.SetCursorPosition(0, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, top);
        }
    }
}
