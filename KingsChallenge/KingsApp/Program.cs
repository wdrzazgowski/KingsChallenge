// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.IO;
using System.Text.Json;
using System.Net;

public class KingsChallenge
{
    static void Main(string[] args)
    {
        string _kingsFileOnline = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
        List<KingRaw> _kings = new List<KingRaw>();

        WebRequest wr = WebRequest.Create(_kingsFileOnline);
        wr.Credentials = CredentialCache.DefaultCredentials;
        HttpWebResponse response = (HttpWebResponse)wr.GetResponse ();
            // Display the status.
        Console.WriteLine (response.StatusDescription);
            // Get the stream containing content returned by the server.
        Stream dataStream = response.GetResponseStream ();
            // Open the stream using a StreamReader for easy access.
        using(StreamReader sr = new StreamReader(dataStream))
        {
            string kings = sr.ReadToEnd();
            // Console.Out.WriteLine(kings);
            _kings = JsonSerializer.Deserialize<List<KingRaw>>(kings);
        }
        Console.Out.WriteLine(_kings.Count);

        KingRaw firstKing = _kings.First();
        Console.Out.WriteLine(firstKing.nm);
    }
}

class KingRaw
{
    public int id { get; set; }
    public string nm { get; set; }
    public string cty { get; set; }
    public string hse { get; set; }
    public string yrs { get; set; }
}
