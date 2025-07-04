using Cryptographer;
using Cryptographer.DecryptionMethods;
using Cryptographer.DecryptionUtils;

class Program
{

    private static List<IDecryptionMethod> methods = new()
    {
        new Reverse(),
        new Atbash(),
        new Base64(),
    };

    private static List<IListDecryptionMethod> listMethods = new()
    {
        new Morse(),
        new Baconian(),
        new KeyboardSubstitution()
    };

    private static int maxDepth = 0;


    public static async Task<List<string>> GetDecrypted(string input, int depth = 1, string lastMethod = "")
    {
        // almost no puzzle has a 1 letter solution due to how easy it is to bruteforce
        if (depth > maxDepth) {
            return new List<string>() { input };
        }

        List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(input);

        List<Task<List<string>>> tasks = new();
        
        // single output ones
        foreach (IDecryptionMethod method in methods)
        {
            // atbashing/reversing twice doesnt make sense
            if (lastMethod == method.Name && (method.Name == "Reverse" || method.Name == "Atbash"))
            {
                continue;
            }

            // decrypt and go deeper
            string output = method.Decrypt(input, analysis);
            // doesnt make sense for a puzzle solution to be only spaces/two letters because of how easily they can be bruteforced
            if (output.Replace(" ", "").Length <= 2)
            {
                continue;
            }

            tasks.Add(Task.Run(() => GetDecrypted(output, depth + 1, method.Name)));
        }

        // multiple output ones
        foreach (IListDecryptionMethod method in listMethods)
        {
            List<string> outputs = method.Decrypt(input, analysis);
            foreach (string output in outputs)
            {
                tasks.Add(Task.Run(() => GetDecrypted(output, depth + 1, method.Name)));
            }
        }

        List<string> decrypted = new();
        List<string>[] decryptedList = await Task.WhenAll(tasks);
        foreach (List<string> list in decryptedList)
            // remove duplicates
            decrypted.AddRange(list.ToHashSet().ToArray());

        return decrypted;
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");
        // get user input
        string input = Utils.GetInput();
        Console.Clear();
        maxDepth = Utils.GetDepth();
        Console.Clear();
        Console.WriteLine("Working...");

        // output
        List<string> outputs = await GetDecrypted(input, 1);

        Console.WriteLine($"Possible outputs:");
        foreach (string output in outputs)
        {
            // dont print duplicates
            float score = StringScorer.Score(output);
            Console.WriteLine($"{output} ({score})");
        }

    }
}