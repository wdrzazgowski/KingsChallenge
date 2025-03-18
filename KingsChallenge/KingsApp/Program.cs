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
        Console.Out.WriteLine("Monarch count: {0}", _kings.Count);

        foreach(KingRaw king in _kings)
            Console.Out.WriteLine(king.ToString());
    }
}

class KingRaw
{
    public int id { get; set; }
    public string nm { get; set; }
    public string cty { get; set; }
    public string hse { get; set; }

    private string _years;
    public string yrs 
    { 
        get
        {
            return _years;
        }
        set
        {
            _years = value;
            ParseReignYears();
        } 
    }
    public int _reignStart;
    public int _reignEnd;

    private void ParseReignYears()
    {
        string[] years = _years.Split('-');
        _reignStart = Int32.Parse(years[0]);
        if(years.Length == 1)
            _reignEnd = _reignStart;
        else
        {
            //Console.Out.WriteLine("Years[1]: {0}", years[1]);
            if(!Int32.TryParse(years[1], out _reignEnd))
                _reignEnd = -1;
            //Console.Out.WriteLine("_reignEnd: {0}", _reignEnd);
        }
    }

    public int ReignLength()
    {
        // Console.Out.WriteLine(_years);
        // Console.Out.WriteLine(_reignStart);
        // Console.Out.WriteLine(_reignEnd);
        return (_reignEnd - _reignStart);
    }

    public override string ToString()
    {
        // return String.Format("King {0}\n\tCity {1}\n\t{House {2}\n\tReign started at {3}\n\tReign ended at {4}\n\tReigned for {5}",
        //     nm,
        //     cty,
        //     hse,
        //     _reignStart,
        //     _reignEnd,
        //     ReignLength());
        int len = ReignLength();
        return String.Format("{0}\n\t{1}\n\t{2}\n\t{3}\n\t{4}\n\t{5}", nm, cty, hse, _reignStart, _reignEnd, len);
    }
}
