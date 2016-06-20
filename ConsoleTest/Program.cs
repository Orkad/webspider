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

        static string[] GetFilter = { "cpu", "id" };
        static string URL = "https://www.cpubenchmark.net/";
        private static WebExtractor extractor;

        static void Main(string[] args)
        {
            
            
            XPathDataDefinition dataDefinition = new XPathDataDefinition();
            dataDefinition.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinition.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            Console.WriteLine("Exploration de " + URL);
            WebExplorer explorer = new WebExplorer(Filter);
            extractor = new WebExtractor(explorer, dataDefinition);
            extractor.SuccessParse += ExtractorOnSuccessParse;
            extractor.FailParse += PageError;
            
            
            try
            {
                explorer.Explore(new Uri(URL));
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Erreur fatale : " + e.Message);
            }
            Console.ReadKey();
        }

        private static void ExtractorOnSuccessParse(WebPage webPage, Dictionary<string, string> dataDictionary)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nouvel objet trouvé !        TOTAL = " + extractor.Data.Count);
            foreach (var field in dataDictionary)
            {
                Console.WriteLine("[" + field.Key + "] = " + field.Value);
            }
        }


        private static void PageError(WebPage obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Erreur : " + obj.Adress.AbsoluteUri);
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
