using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            DataDefinition dataDefinition = new DataDefinition();
            dataDefinition.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinition.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            MiningContext context = new MiningContext("https://www.cpubenchmark.net/", new[] { "cpu", "id" },dataDefinition);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.Run();


            
            Console.ReadKey();
            context.SaveDataMining("data.mining");
        }

        private static void ContextOnExplore(WebPageOld webPage)
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
