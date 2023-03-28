using SharpToken.Lib;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var name = "cl100k_base";
        var encoding = GptEncoding.GetEncoding(name);

        Console.WriteLine("qwe");
        var z = "おめでとうHello😊😊😊😊😊😊 worldqwer^";
        Console.WriteLine("List contents (option 2): " + string.Join(", ", encoding.Encode(z)));
        Console.WriteLine(encoding.Decode(encoding.Encode(z)));
        Console.ReadLine();
    }
}
