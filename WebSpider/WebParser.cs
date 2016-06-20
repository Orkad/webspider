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
            XDocument doc = XDocument.Parse(html);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var field in FieldXPath)
            {
                dictionary.Add(field.Key,doc.Root.XPathSelectElement(field.Value).Value);
            }
            return dictionary;
        } 
    }
}
