// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.IO;
using System.Text.Json;
using System.Net;
using System.Xml;
using System.Reflection;
using System;

using log4net; 

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace KingsChallenge
{
    public class KingsChallenge
    {
        private static readonly log4net.ILog? logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            string _kingsFileOnline = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
           
            try
            {
                ConfigureLogger();
            }
            catch
            {
                // not very nice... but if log4net config is not found the applciation will continue
                Console.Error.WriteLine("Could not initialize log4net logger - continuing without.");
            }

            logger.Debug("Creating MonarchDataProvider object");
            MonarchDataProvider mdp = new MonarchDataProvider(logger);
            
            List<Monarch> _kings = mdp.GetMonarchData(_kingsFileOnline).Result;

            MonarchListUtils mlUtils = new MonarchListUtils(_kings, logger);

            int monarchCount = mlUtils.GetMonarchCount();
            Console.Out.WriteLine("Monarch count: {0}", monarchCount);

            Monarch longestReigningMonarch = mlUtils.GetLongestReiningMonarch();
            Console.Out.WriteLine("Longest reigning monarch: {0}, ruled for {1} years\n\tAre we not shamefuly ignoring King Charles?", 
                longestReigningMonarch.Name, 
                longestReigningMonarch.ReignLength());


            var longestReiningHouse = mlUtils.GetLongestReigningHouse();  // ?? "Longet reigning house not found - possible error in data processing."
            Console.Out.WriteLine($"Longest reigning house: {longestReiningHouse.Item1} reigned for {longestReiningHouse.Item2} years");


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
        public async Task<List<Monarch>> GetMonarchData(string monarchUri)
        {
            using(HttpClient c = new HttpClient())
            {
                _logger.Debug($"Trying to fetch monarch data from {monarchUri}");
                HttpResponseMessage response = await c.GetAsync(monarchUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //_logger.Debug($"Read response {responseBody}");
                List<MonarchDto> mdtoList = JsonSerializer.Deserialize<List<MonarchDto>>(responseBody);
                
                List<Monarch> monarchs = new List<Monarch>();
                foreach(MonarchDto mdto in mdtoList)
                {
                    Monarch m = new Monarch(mdto, _logger);
                    m.ParseReignYears();
                    monarchs.Add(m);
                }
                return monarchs;
            }
        }
    }

    class MonarchListUtils
    {
        List<Monarch> _monarchList;
        ILog _logger;
        public MonarchListUtils(List<Monarch> monarchList, ILog logger)
        {
            _logger = logger;
            _monarchList = monarchList;
        }

        public int GetMonarchCount()
        {
            if(_monarchList == null)
                return 0;

            return _monarchList.Count();
        }

        public Monarch GetLongestReiningMonarch()
        {
            Monarch lrMonarch = _monarchList.MaxBy(m => m.ReignLength());
            return lrMonarch;
        }

        public (string, int) GetLongestReigningHouse()
        {
            if(_monarchList == null)
                return ("", 0);

            IEnumerable<IGrouping<string?, Monarch>> houses = _monarchList.GroupBy( monarch => monarch._monarchData.hse);
            
            List<(string, int)> houseRules = new List<(string, int)>();
            foreach(IGrouping<string?, Monarch> house in houses)
            {
                int ruledFor = house.Sum( h => h.ReignLength());
                _logger.Debug($"House {house.Key} ruled for {ruledFor} years.");  
                houseRules.Add((house.Key, ruledFor));
            }
            //return (houses.MaxBy( house => house.Count()).Key, 99);
            return houseRules.MaxBy(h=>h.Item2);
        }

        public string GetMostCommonFirstName()
        {
            if(_monarchList ==null)
                return null;

            IEnumerable<IGrouping<string, Monarch>> fNames = _monarchList.GroupBy( monarch => monarch.FirstName());

            return fNames.MaxBy( fName => fName.Count()).Key;
        }
    }

    public class MonarchDto
    {
        public int id { get; set; }
        public string? nm { get; set; }
        public string? cty { get; set; }
        public string? hse { get; set; }
        public string? yrs { get; set; }
    }

    public class Monarch
    {
        ILog _logger;
        public MonarchDto _monarchData;

        public Monarch()
        {

        }

        public Monarch(MonarchDto mdto, ILog logger)
        {
            _logger = logger;
            _monarchData = mdto;
        }        

        public int _reignStart;
        public int _reignEnd;

        public string Name
        {
            get
            {
                return _monarchData.nm;
            }
        }

        public void ParseReignYears()
        {  
            try
            {
                string[] years = _monarchData.yrs.Split('-');
                _reignStart = Int32.Parse(years[0]);
                if(years.Length == 1)
                    _reignEnd = _reignStart;
                else
                {
                    if(!Int32.TryParse(years[1], out _reignEnd))
                        _reignEnd = DateTime.Now.Year;
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"Cound not parse reign years for monarch {_monarchData.nm}", ex);
                _reignStart = 0;
                _reignEnd = 0;
                return; 
            }
        }

        public int ReignLength()
        {
            return (_reignEnd - _reignStart);
        }

        public string FirstName()
        {
            string[] parts = _monarchData.nm.Split(' ');
            return parts[0];
        }
    }
}