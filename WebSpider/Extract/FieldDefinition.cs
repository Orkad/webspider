using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WebSpiderLib.Extract
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
            if (value == null)
                return null;
            value = Regex.Replace(value, @"\t|\r", "");
            value = Regex.Replace(value, @"\n", " ");
            return new Field(Name, value);
        }
    }
}