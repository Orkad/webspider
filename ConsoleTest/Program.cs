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

        static void Main(string[] args)
        {
            WebCrawler crawler = new WebCrawler(Filter, WriteGreen, WriteRed);
            try
            {
                
                crawler.Explore(new Uri("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources"));
                Console.ReadKey();
            }
            catch (Exception)
            {
                // ignored
            }
            WriteGreen("Fin de l'exploration : " + crawler.ExploredPages + " pages explorées");
            Console.ReadKey();
        }

        private static void OnPageLoaded(WebPage obj)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Chargement de la page terminé");
        }


        private static void WriteRed( string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
        }

        private static void WriteGreen(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
        }

        private static bool Filter(Uri uri)
        {
            string href = uri.AbsoluteUri;
            if (href.Contains("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources"))
                if (!href.Contains("?") || href.Contains("?page") && !href.Contains("&"))
                    return true;
            return false;
        }
    }
}
