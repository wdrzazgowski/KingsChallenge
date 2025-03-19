// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.IO;
using System.Text.Json;
using System.Net;
using System.Xml;
using System.Reflection;

using log4net; 

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace KingsChallenge
{
    public class KingsChallenge
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            string _kingsFileOnline = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
           
            ConfigureLogger();

            logger.Debug("Creating MonarchDataProvider object");
            MonarchDataProvider mdp = new MonarchDataProvider(logger);
            
            List<KingRaw> _kings = mdp.GetMonarchData(_kingsFileOnline).Result;

            MonarchListUtils mlUtils = new MonarchListUtils(_kings, logger);

            int monarchCount = mlUtils.GetMonarchCount();
            Console.Out.WriteLine("Monarch count: {0}", monarchCount);

            KingRaw longestReigningMonarch = mlUtils.GetLongestReiningMonarch();
            Console.Out.WriteLine("Longest reigning monarch: {0}, ruled for {1} years\n\tAre we not shamefuly ignoring King Charles?", longestReigningMonarch.nm, longestReigningMonarch.ReignLength());


            string longestReiningHouse = mlUtils.GetLongestReiningHouse();
            Console.Out.WriteLine("Longest reigning house: {0}", longestReiningHouse);


            string mostCommonFirstName = mlUtils.GetMostCommonFirstName();
            Console.Out.WriteLine("Most common first name: {0}", mostCommonFirstName);
        }

        static void ConfigureLogger()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                        typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
    }

    class MonarchDataProvider
    {
        ILog _logger;
        public MonarchDataProvider(ILog logger)
        {
            _logger = logger;
        }
        public async Task<List<KingRaw>> GetMonarchData(string monarchUri)
        {
            using(HttpClient c = new HttpClient())
            {
                _logger.Debug($"Trying to fetch monarch data from {monarchUri}");
                HttpResponseMessage response = await c.GetAsync(monarchUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.Debug($"Read response {responseBody}");
                return JsonSerializer.Deserialize<List<KingRaw>>(responseBody);
            }
        }
    }

    class MonarchListUtils
    {
        List<KingRaw> _monarchList;
        ILog _logger;
        public MonarchListUtils(List<KingRaw> monarchList, ILog logger)
        {
            _logger = logger;
            _monarchList = monarchList;
        }

        public int GetMonarchCount()
        {
            return _monarchList.Count();
        }

        public KingRaw GetLongestReiningMonarch()
        {
            KingRaw? lrMonarch = _monarchList.MaxBy( m => m.ReignLength());
            return lrMonarch;
        }

        public string GetLongestReiningHouse()
        {
            IEnumerable<IGrouping<string?, KingRaw>> houses = _monarchList.GroupBy( monarch => monarch.hse);
            
            return houses.MaxBy( house => house.Count()).Key;
        }

        public string GetMostCommonFirstName()
        {
            IEnumerable<IGrouping<string, KingRaw>> fNames = _monarchList.GroupBy( monarch => monarch.FirstName());

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
                if(!Int32.TryParse(years[1], out _reignEnd))
                    _reignEnd = DateTime.Now.Year;
            }
        }

        public int ReignLength()
        {
            return (_reignEnd - _reignStart);
        }

        public string FirstName()
        {
              string[] parts = nm.Split(' ');
            return parts[0];
        }
    }
}