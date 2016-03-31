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
                Filter = "data.gouv.fr",
                Log = Console.Out
            };
            spider.Run("http://data.gouv.fr");
            Console.ReadKey();
        }
    }
}
