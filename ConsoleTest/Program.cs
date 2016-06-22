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
            DataDefinition dataDefinitionCpuBenchmark = new DataDefinition();
            dataDefinitionCpuBenchmark.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            dataDefinitionCpuBenchmark.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");
            MiningTester tester = new MiningTester(dataDefinitionCpuBenchmark);
            tester.TestResult += TesterOnTestResult;
            tester.TryUri(new Uri("https://www.cpubenchmark.net/cpu.php?cpu=Intel+Xeon+E5-2679+v4+%40+2.50GHz&id=2805"));
            Console.ReadKey();

            MiningContext context = new MiningContext("https://www.cpubenchmark.net/", new[] { "cpu", "id" }, dataDefinitionCpuBenchmark);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.ExploreError += ContextOnExploreError;
            data = context.Extractor.Data;
            context.Run();


            
            Console.ReadKey();
            context.SaveDataMining("data.mining");
        }

        private static void TesterOnTestResult(Data d)
        {
            if (d != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Données extraites");
                foreach (var field in d.Fields)
                {
                    Console.WriteLine("[" + field.Name + "] = " + field.Value);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Pas de données");
            }
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
