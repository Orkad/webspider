using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSpiderLib;
using HtmlAgilityPack;

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
                TryAgainOnFail = true
            };
            spider.ExploreEvent += ExtractRessource;
            spider.Run("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources");
            Console.ReadKey();
        }

        public class DofusRessource
        {
            public int Level { get; set; }

            public string Nom { get; set; }

            public string Categorie { get; set; }

            public string Description { get; set; }
        }

        static List<DofusRessource> ressources = new List<DofusRessource>(); 

        private static void ExtractRessource(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            try
            {
                DofusRessource res = new DofusRessource();
                res.Nom = XpathSearch(doc, "//h1[@class='ak-return-link']");
                res.Categorie = XpathSearch(doc, "//div[@class='ak-encyclo-detail-type col-xs-6']/span");
                res.Description = XpathSearch(doc, "//div[@class='col-sm-9']//div[@class='ak-panel-content']");
                string LevelString = XpathSearch(doc, "//div[@class='ak-encyclo-detail-level col-xs-6 text-right']").Replace("Niveau :","");
                res.Level = int.Parse(LevelString);
                Console.WriteLine("Ressource ajoutée (total = " + ressources.Count + ")");
                ressources.Add(res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Not a ressource");
            }
        }

        private static string XpathSearch(HtmlDocument doc, string xpath)
        {
            if (string.IsNullOrEmpty(xpath) || doc.DocumentNode.SelectSingleNode(xpath) == null)
                return null;
            return doc.DocumentNode.SelectSingleNode(xpath).InnerText.Trim();
        }
    }
}
