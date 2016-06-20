using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    public class WebParser
    {
        public readonly Dictionary<string, string> FieldXPath = new Dictionary<string, string>();

        public WebParser(Dictionary<string, string> fieldXPath)
        {
            FieldXPath = fieldXPath;
        }

        public Dictionary<string, string> TryParse(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var field in FieldXPath)
            {
                string paramName = field.Key;
                string value = doc.DocumentNode.SelectSingleNode(field.Value)?.InnerText;
                if (value == null)
                    throw new Exception("La page web ne correspond pas à la definition de l'objet");
                dictionary.Add(paramName,value);
            }
            return dictionary;
        } 
    }
}
