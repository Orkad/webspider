using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSpiderLib;


namespace ConsoleTest
{
    class Program
    {
        public static string[] GetFilter = {"cpu"};
        public const string URL = "https://www.cpubenchmark.net/";
        public static List<Dictionary<string, string>> Data; 
        public static WebCrawler crawler = new WebCrawler(Filter)
        {
            OnPageLoaded = PageLoaded,
            OnPageFound = PageFound,
            OnPageError = PageError,
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Exploration de " + URL);
            
            try
            {
                crawler.Explore(new Uri(URL));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur fatale : " + e.Message);
            }
            Console.ReadKey();
        }

        private static void PageError(WebPage obj)
        {
            CrawlerStats();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Erreur : " + obj.Adress.AbsoluteUri);
        }

        private static void PageFound(Uri uri)
        {
            CrawlerStats();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Requète : " + uri.AbsoluteUri);
            
        }

        private static void PageLoaded(WebPage obj)
        {
            CrawlerStats();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Réponse : " + obj.Adress.AbsoluteUri);
            WebParser parser = new WebParser(new Dictionary<string, string>());
            parser.FieldXPath.Add("id", "id");
            Data.Add(parser.TryParse(obj.Html));
        }

        private static void CrawlerStats()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            if (crawler.LoadingPages == 0)
                Console.WriteLine("Aucune requète en cours");
            Console.WriteLine("Requète en cours : " + crawler.LoadingPages);
        }

        private static bool Filter(Uri uri)
        {
            string href = uri.AbsoluteUri;
            var getParam = uri.ParseQueryString();
            if (!href.Contains('#') && href.Contains(URL))
            {
                if (getParam.Count != 0)
                {
                    foreach (var param in getParam)
                    {
                        foreach (var filter in GetFilter)
                        {
                            if (param.Key == filter)
                                return true;
                        }
                    }
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
