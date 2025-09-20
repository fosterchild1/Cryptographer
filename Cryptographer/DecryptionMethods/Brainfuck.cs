using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Brainfuck : IDecryptionMethod
    {

        public List<string> Decrypt(string input, StringInfo info)
        {
            int pointer = 0;
            byte[] tape = new byte[1024];
            Stack<int> loops = new();

            StringBuilder output = new();

            int i = -1;
            while (true)
            {
                i++;
                if (i >= input.Length)
                    break;

                char c = input[i];

                // this might be some of the shittest code in this project
                switch(c) {
                    case '>': pointer++; break;
                    case '<': pointer--; break;
                    case '+': tape[pointer]++; break;
                    case '-': tape[pointer]--; break;

                    case '.':
                        output.Append((char)(tape[pointer])); 
                        break; // add to output

                    case '[':
                        // if value is zero, skip to end of the loop
                        if (tape[pointer] == 0)
                        {
                            while (input[i] != ']') i++;
                            break;
                        }

                        loops.Push(i);
                        break;

                    case ']':
                        if (tape[pointer] != 0)
                        {
                            i = loops.Peek();
                            break;
                        }

                        if (loops.Count == 0) return new(); // invalid bf string

                        loops.Pop();
                        break;
                }
            }

            return new() { output.ToString() };
        }

        private HashSet<char> bfChars = new() { '>', '<', '+', '-', '.', '[', ']' };
        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count <= 2 || analysis.Count > bfChars.Count) return 1;

            foreach (char c in input)
            {
                if (!bfChars.Contains(c)) return 1;
            }

            return 0;
        }

        public string Name { get { return "Brainfuck"; } }
    }
}
