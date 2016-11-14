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
using WebSpiderLib.Extract.Persistence;


namespace ConsoleTest
{
    class Program
    {
        private static int dataCount;
        private static int pageCount;
        private static int errorCount;
        static readonly DateTime StartDateTime = DateTime.Now;

        private static MiningContext context;
        private static List<Data> data;
        static void Main(string[] args)
        {
            var CPUBM_URI = new Uri("http://cpubenchmark.net/");

            ExplorationFilter CPUBM_UriValidator = new ExplorationFilter();
            CPUBM_UriValidator.ValidBaseUriList.Add(CPUBM_URI);
            CPUBM_UriValidator.ValidGetList.Add("cpu");
            CPUBM_UriValidator.ValidGetList.Add("id");

            DataDefinition CPUBM_DataDef = new DataDefinition("cpubm");
            CPUBM_DataDef.AddXPathMatching("cpu", "//table[@class=\"desc\"]//span");
            CPUBM_DataDef.AddXPathMatching("benchmark", "//table[@class=\"desc\"]//tr[2]/td[2]/span");

            SqlDataPersistence persistence = new SqlDataPersistence("server=127.0.0.1;uid=root;pwd=;database=test;", CPUBM_DataDef);
            context = new MiningContext(CPUBM_UriValidator.Validate, CPUBM_DataDef, persistence);
            context.Extract += ContextOnExtract;
            context.Explore += ContextOnExplore;
            context.ExploreError += ContextOnExploreError;
            context.Done += () => Console.WriteLine("\n\nExploration terminée");
            data = context.Extractor.Data;
            context.Run(CPUBM_URI);

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
            ShowStatistics();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exploration : " + uri.AbsoluteUri);
        }

        private static void ContextOnExplore(WebPage webPage)
        {
            pageCount++;
            ShowStatistics();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Exploration : " + webPage.Adress.AbsoluteUri);
        }

        private static void ContextOnExtract(Data resultData)
        {
            dataCount++;
            ShowStatistics();
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
            Console.WriteLine("Requètes en attente de réponse : " + context.Explorator.remainingPages);
            Console.WriteLine("Taille du buffer d'adresse : " + context.Explorator.bufferSize);
            Console.WriteLine();
        }
    }
}
