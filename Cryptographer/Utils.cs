using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptographer
{
    internal class Utils
    {
        public static bool loggingEnabled = true;

        public static string GetInput()
        {
            Console.WriteLine("Please input the text you want to decrypt:");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Input can't be empty. Please try again.");
                return GetInput();
            }

            return input;
        }

        public static int GetDepth()
        {
            Console.WriteLine("Please input the maximum depth of the search:");

            //is something?
            string? inputLine = Console.ReadLine();
            if (string.IsNullOrEmpty(inputLine))
            {
                Console.WriteLine("Depth can't be empty. Please try again.");
                return GetDepth();
            }

            // is integer?
            bool isInt = int.TryParse(inputLine, out int input);
            if (!isInt)
            {
                Console.WriteLine("Input isn't an integer. Please try again.");
                return GetDepth();
            }

            return Math.Max(input, 1);
        }

        public static Dictionary<string, Stopwatch> timers = new();
        public static void StartTimer(string name)
        {
            if (!loggingEnabled) { return; }
            if (timers.ContainsKey(name)) { return; }

            timers.Add(name, Stopwatch.StartNew());
        }

        public static void StopTimer(string name)
        {
            if (!loggingEnabled) { return; }

            Stopwatch? timer;
            if (!timers.TryGetValue(name, out timer)) { return; }

            timer.Stop();
            timers.Remove(name);

            Console.WriteLine(timer.Elapsed.Nanoseconds);
        }
    }
}
