using System;
using System.Collections.Generic;
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
            WebSpider spider = new WebSpider
            {
                Filter = "http://www.dofus.com/fr/mmorpg/encyclopedie/ressources",
                Log = Console.Out,
                TryAgainOnFail = false
            };
            spider.Run("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources");
            Console.ReadKey();
        }
    }
}
