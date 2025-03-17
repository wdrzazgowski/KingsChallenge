namespace KingsChallenge;

using System.IO;
using System.Text.Json;

public class KingsChallenge
{
    static void Main(string[] args)
    {
        List<King> _kings = new List<King>();

        using(StreamReader sr = new StreamReader("https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings"))
        {
            string kings = sr.ReadToEnd();
            _kings = JsonSerializer.Deserialize<List<King>>(kings);
        }
        Console.Out.WriteLine(_kings);
    }
}

public class King
{
    int _id;
    string _nm;
    string _cty;
    string _hse;
    string _yrs;
}
