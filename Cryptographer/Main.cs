using Cryptographer.DecryptionMethods;

enum DecryptionMethod
{
    Reverse,
}

class Program
{
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

   static void Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");
        string input = GetInput();

        var methods = new List<IDecryptionMethod>
        {
            new Reverse(),
        };

        foreach (IDecryptionMethod method in methods)
        {
            Console.WriteLine($"{method.Name}: {method.Decrypt(input)}");
        }
    }
}