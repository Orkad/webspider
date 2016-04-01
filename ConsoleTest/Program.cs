using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSpiderLib;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using static System.String;

namespace ConsoleTest
{
    class Program
    {
        private static string currentUrl { get; set; } = "Vide";
        private static readonly WebSpider spider = new WebSpider();

        static void Main(string[] args)
        {
            spider.Filter = Filter;
            spider.HtmlEvent += ExtractRessource;
            spider.CurrentUriChange += OnUriChange;
            spider.Run("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources");
            Console.ReadKey();
        }

        private static void ConsoleRefresh()
        {
            Console.Clear();
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("-----------------WebSpider---------------");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("");
            Console.WriteLine(" Url courante        => " + currentUrl);
            Console.WriteLine(" Pages explorées     => " + (spider.Links.MemoryCount - spider.Links.QueueCount));
            Console.WriteLine(" Pages restantes     => " + spider.Links.QueueCount);
            Console.WriteLine(" Pages total         => " + spider.Links.MemoryCount);
            Console.WriteLine(" Ressources trouvées => " + ressources.Count);
        }

        private static void OnUriChange(Uri uri)
        {
            currentUrl = uri.AbsoluteUri;
            ConsoleRefresh();
        }

        private static bool Filter(string href)
        {
            if (href.Contains("http://www.dofus.com/fr/mmorpg/encyclopedie/ressources"))
                if (!href.Contains("?") || href.Contains("?page") && !href.Contains("&"))
                    return true;
            return false;
        }

        public class DofusRessource
        {
            private static string TableName = "ressource";
            public int Level { get; set; }

            public string Nom { get; set; }

            public string Categorie { get; set; }

            public string Description { get; set; }

            public string ToSQL()
            {
                if (IsNullOrWhiteSpace(Nom) || IsNullOrWhiteSpace(Categorie) || IsNullOrWhiteSpace(Description))
                    return "";
                return "INSERT OR REPLACE INTO " + TableName + " VALUES('" + Nom + "'," + Level + ",'" + Categorie +
                           "','" + Description + "')";
            }
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
                ressources.Add(res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Not a ressource");
            }
            ConsoleRefresh();
        }

        private static string XpathSearch(HtmlDocument doc, string xpath)
        {
            if (IsNullOrEmpty(xpath) || doc.DocumentNode.SelectSingleNode(xpath) == null)
                return null;
            return doc.DocumentNode.SelectSingleNode(xpath).InnerText.Trim();
        }
    }
}
