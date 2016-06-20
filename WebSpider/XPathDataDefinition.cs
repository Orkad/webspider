using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    public class XPathDataDefinition
    {
        private readonly Dictionary<string, string> Matching = new Dictionary<string, string>();

        public void AddXPathMatching(string paramName, string xpathRequest)
        {
            Matching.Add(paramName,xpathRequest);
        }

        public Dictionary<string, string> ParseWebPage(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var field in Matching)
            {
                string paramName = field.Key;
                string value = doc.DocumentNode.SelectSingleNode(field.Value)?.InnerText.Trim();
                if (value == null)
                    throw new Exception("La page web ne correspond pas à la definition de l'objet");
                dictionary.Add(paramName, value);
            }
            return dictionary;
        }
    }
}