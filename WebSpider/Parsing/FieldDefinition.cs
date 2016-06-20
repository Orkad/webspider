using System;
using HtmlAgilityPack;

namespace WebSpiderLib.Parsing
{
    public class FieldDefinition
    {
        public FieldDefinition(string name, string xpath)
        {
            Name = name;
            XPath = xpath;
        }

        public string Name { get; }
        public string XPath { get; }

        public Field Parse(HtmlDocument document)
        {
            string value = document.DocumentNode.SelectSingleNode(XPath)?.InnerText.Trim();
            if(value == null)
                throw new Exception("Pas de valeur trouvé pour la requète XPath");
            return new Field(Name, value);
        }
    }
}