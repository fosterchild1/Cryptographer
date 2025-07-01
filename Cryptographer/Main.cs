using Cryptographer;
using Cryptographer.DecryptionMethods;

enum DecryptionMethod
{
    Reverse,
}

class Program
{

    public static List<IDecryptionMethod> methods = new()
    {
        new Reverse(),
        new Atbash(),
    };

    public static int maxDepth = 0;


    public static List<string> GetDecrypted(string input, int depth = 1)
    {
        // almost no puzzle has a 1 letter solution due to how easy it is to bruteforce
        if (depth > maxDepth || input.Length <= 1) {
            Console.WriteLine("exiting");
            return new List<string>() {
               input
            };
        }

        List<string> decrypted = new();

        foreach (IDecryptionMethod method in methods)
        {
            Console.WriteLine(method.Name);
            Dictionary<char, int> analysis = FrequencyAnalysis.AnalyzeFrequency(input);

            string output = method.Decrypt(input);
            decrypted.AddRange(GetDecrypted(output, depth + 1));
        }

        return decrypted;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");
        string input = Utils.GetInput();
        Console.Clear();
        maxDepth = Utils.GetDepth();

        List<string> outputs = GetDecrypted(input, 1);

        Console.WriteLine($"Possible outputs ({outputs.Count.ToString()}):");
        foreach (string output in outputs)
        {
            Console.WriteLine(output);
        }

    }
}