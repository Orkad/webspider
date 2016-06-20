using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSpiderLib;
using WebSpiderLib.Parsing;


namespace ConsoleTest
{
    class Program
    {
        static string URL = "https://www.cpubenchmark.net/";

        static void Main(string[] args)
        {
            DataDefinition dataDefinition = new DataDefinition();
            dataDefinition.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinition.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            MiningContext context = new MiningContext(URL, new[] { "cpu", "id" },dataDefinition);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.Run();


            
            Console.ReadKey();
            context.SaveDataMining("data.mining");
        }

        private static void ContextOnExplore(WebPage webPage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Exploration : " + webPage.Adress.AbsoluteUri);
        }

        private static void ContextOnExtract(WebPage extractedPage, Data resultData)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nouvel objet trouvé !");
            foreach (var field in resultData.Fields)
            {
                Console.WriteLine("[" + field.Name + "] = " + field.Value);
            }
        }


    }
}
