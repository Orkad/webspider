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

            DataDefinition RDC_DataDef = new DataDefinition();
            RDC_DataDef.AddXPathMatching("produit", "//span[@class='productName']");
            RDC_DataDef.AddXPathMatching("ref", "//h3[@class='ficheProduit_reference']");
            RDC_DataDef.AddXPathMatching("price", ".//*[@class='newPrice']");

            ExplorationFilter RDC_UriValidator = new ExplorationFilter();
            RDC_UriValidator.ValidBaseUriList.Add(new Uri("http://www.rueducommerce.fr/Composants/Processeur/"));
            RDC_UriValidator.ValidBaseUriList.Add(new Uri("http://www.rueducommerce.fr/Composants/54-Comparatif-Processeur/"));

            MiningContext context = new MiningContext(new Uri("http://www.rueducommerce.fr/Composants/54-Comparatif-Processeur/"), RDC_UriValidator.Validate, RDC_DataDef);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.ExploreError += ContextOnExploreError;
            context.Done += () => Console.WriteLine("\n\nExploration terminée");
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
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
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
