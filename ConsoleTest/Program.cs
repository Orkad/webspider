using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSpiderLib;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;


namespace ConsoleTest
{
    class Program
    {
        private static List<Data> data;
        static void Main(string[] args)
        {
            DataDefinition dataDefinition = new DataDefinition();
            dataDefinition.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinition.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            MiningContext context = new MiningContext("https://www.cpubenchmark.net/", new[] { "cpu", "id" },dataDefinition);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.ExploreError += ContextOnExploreError;
            data = context.Extractor.Data;
            context.Run();


            
            Console.ReadKey();
            context.SaveDataMining("data.mining");
        }

        private static void ContextOnExploreError(Uri uri)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exploration : " + uri.AbsoluteUri);
        }

        private static void ContextOnExplore(WebPage webPage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Exploration : " + webPage.Adress.AbsoluteUri);
        }

        private static void ContextOnExtract(Data resultData)
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
