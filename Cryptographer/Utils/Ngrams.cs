using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

public static class Ngrams
{
    private static readonly JsonSerializerOptions options = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    private static byte[] json = File.ReadAllBytes($"{AppContext.BaseDirectory}/resources/trigrams.json");
    public static byte[] qjson = File.ReadAllBytes($"{AppContext.BaseDirectory}/resources/quadgrams.json");

    public static Dictionary<string, float>? trigrams;

    public static Dictionary<string, float>? quadgrams;

    public static void Wake()
    {
        trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json, options);
        quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson, options);
    } // init
}