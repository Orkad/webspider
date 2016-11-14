using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSpiderLib;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;


namespace ConsoleTest
{
    class Program
    {
        private static int dataCount;
        private static int pageCount;
        private static int errorCount;
        static readonly DateTime StartDateTime = DateTime.Now;

        
        private static List<Data> data;
        static void Main(string[] args)
        {
            var CPUBM_URI = new Uri("http://cpubenchmark.net/");

            ExplorationFilter CPUBM_UriValidator = new ExplorationFilter();
            CPUBM_UriValidator.ValidBaseUriList.Add(CPUBM_URI);
            CPUBM_UriValidator.ValidGetList.Add("cpu");
            CPUBM_UriValidator.ValidGetList.Add("id");

            DataDefinition CPUBM_DataDef = new DataDefinition();
            CPUBM_DataDef.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            CPUBM_DataDef.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");


            DataDefinition RDC_DataDef = new DataDefinition();
            RDC_DataDef.AddXPathMatching("produit", "//span[@class='productName']");
            RDC_DataDef.AddXPathMatching("ref", "//h3[@class='ficheProduit_reference']");
            RDC_DataDef.AddXPathMatching("price", ".//*[@class='newPrice']");

            ExplorationFilter RDC_UriValidator = new ExplorationFilter();
            RDC_UriValidator.ValidBaseUriList.Add(CPUBM_URI);

            //MiningContext context = new MiningContext(RDC_URI, RDC_UriValidator.Validate, RDC_DataDef);
            MiningContext context = new MiningContext(CPUBM_URI, CPUBM_UriValidator.Validate, CPUBM_DataDef);
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
            errorCount++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exploration : " + uri.AbsoluteUri);
        }

        private static void ContextOnExplore(WebPage webPage)
        {
            pageCount++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Exploration : " + webPage.Adress.AbsoluteUri);
        }

        private static void ContextOnExtract(Data resultData)
        {
            dataCount++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Nouvel objet trouvé !");
            foreach (var field in resultData.Fields)
            {
                Console.WriteLine("[" + field.Name + "] = " + field.Value);
            }
        }

        private static void ShowStatistics()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------STATS----------");
            Console.WriteLine("Pages explorées : " + pageCount + " (" + (pageCount / (DateTime.Now - StartDateTime).TotalSeconds).ToString("F") + "/s)");
            Console.WriteLine("Erreurs d'exploration : " + errorCount + " (" + (errorCount / (DateTime.Now - StartDateTime).TotalSeconds).ToString("F") + "/s)");
            Console.WriteLine("Données trouvées : " + dataCount + " (" + (dataCount / (DateTime.Now - StartDateTime).TotalSeconds).ToString("F") + "/s)");
        }
    }
}
