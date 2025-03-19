// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.IO;
using System.Text.Json;
using System.Net;

namespace KingsChallenge
{
    public class KingsChallenge
    {
        static void Main(string[] args)
        {
            string _kingsFileOnline = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";

            MonarchDataProvider dp = new MonarchDataProvider();
            
            List<KingRaw> _kings = dp.GetMonarchData(_kingsFileOnline);

            MonarchListUtils mlUtils = new MonarchListUtils(_kings);

            int monarchCount = mlUtils.GetMonarchCount();
            Console.Out.WriteLine("Monarch count: {0}", monarchCount);

            KingRaw longestReigningMonarch = mlUtils.GetLongestReiningMonarch();
            Console.Out.WriteLine("Longest reigning monarch: {0}, ruled for {1} years\n\tAre we not shamefuly ignoring King Charles?", longestReigningMonarch.nm, longestReigningMonarch.ReignLength());


            string longestReiningHouse = mlUtils.GetLongestReiningHouse();
            Console.Out.WriteLine("Longest reigning house: {0}", longestReiningHouse);


            string mostCommonFirstName = mlUtils.GetMostCommonFirstName();
            Console.Out.WriteLine("Most common first name: {0}", mostCommonFirstName);
        }
    }

    class MonarchDataProvider
    {
        public List<KingRaw> GetMonarchData(string monarchUri)
        {
            WebRequest wr = WebRequest.Create(monarchUri);
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
                return JsonSerializer.Deserialize<List<KingRaw>>(kings);
            }
        }
    }

    class MonarchListUtils
    {
        List<KingRaw> _monarchList;

        public MonarchListUtils(List<KingRaw> monarchList)
        {
            _monarchList = monarchList;
        }

        public int GetMonarchCount()
        {
            return _monarchList.Count();
        }

        public KingRaw GetLongestReiningMonarch()
        {
            KingRaw lrMonarch = _monarchList.MaxBy( m => m.ReignLength());
            return lrMonarch;
        }

        public string GetLongestReiningHouse()
        {
            IEnumerable<IGrouping<string, KingRaw>> houses = _monarchList.GroupBy( monarch => monarch.hse);
            
            // foreach(var houseGroup in houses)
            // {
            //     Console.Out.WriteLine($"House: {houseGroup.Key}, monarchs: {houseGroup.Count()}");
            // }

            return houses.MaxBy( house => house.Count()).Key;
        }

        public string GetMostCommonFirstName()
        {
            IEnumerable<IGrouping<string, KingRaw>> fNames = _monarchList.GroupBy( monarch => monarch.FirstName());

            // foreach(var fnGroup in fNames)
            // {
            //     Console.Out.WriteLine($"First Name: {fnGroup.Key}, monarchs: {fnGroup.Count()}");
            // }
            return fNames.MaxBy( fName => fName.Count()).Key;
        }
    }

    public class KingRaw
    {
        public int id { get; set; }
        public string? nm { get; set; }
        public string? cty { get; set; }
        public string? hse { get; set; }

        private string? _years;
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
                    _reignEnd = DateTime.Now.Year;
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

        public string FirstName()
        {
            string[] parts = nm.Split(' ');
        
            return parts[0];
        }

        public override string ToString()
        {
            int len = ReignLength();
            return String.Format("{0}\n\t{1}\n\t{2}\n\t{3}\n\t{4}\n\t{5}", nm, cty, hse, _reignStart, _reignEnd, len);
        }
    }
}