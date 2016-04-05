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
            spider.Filter = Filter;
            
            ConsoleKey key;
            do
            {
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.Z)
                    spider.Stop();
                if(key == ConsoleKey.A)
                    spider.Start("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources");
                if(key == ConsoleKey.E)
                    spider.Pause();
                if(key == ConsoleKey.R)
                    spider.Continue();
            } while (key != ConsoleKey.Escape);
            spider.Stop();
        }

        private static void SpiderOnLog(string s)
        {
            Console.WriteLine("WebSpider( Requetes : " + spider.RequestCount + " | Réponses : " + spider.ResponseCount + ") " + s);
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
