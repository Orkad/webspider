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
        private static readonly WebSpider spider = new WebSpider();

        static void Main(string[] args)
        {
            spider.Log += SpiderOnLog;
            spider.ErrorLog += SpiderOnErrorLog;
            spider.Filter = Filter;
            
            ConsoleKey key;
            SpiderOnLog(null,"Appuyez sur une touche (A) Lancer | (Z) Stopper | (Esc) Quitter");
            do
            {
                key = Console.ReadKey().Key;
                Console.Clear();
                if (key == ConsoleKey.Z)
                    spider.Stop();
                if (key == ConsoleKey.A)
                    ;//spider.Start("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources");
            } while (key != ConsoleKey.Escape);
            spider.Stop();
        }

        private static void SpiderOnErrorLog(object sender, string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
        }

        private static void SpiderOnLog(object sender, string s)
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
