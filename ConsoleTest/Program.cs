using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orkad.WebSpider;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            WebSpider spider = new WebSpider("fr/mmorpg/encyclopedie/ressources", "http://www.dofus.com/fr/mmorpg/encyclopedie/ressources", Console.Out);
            Console.ReadKey();
        }
    }
}
