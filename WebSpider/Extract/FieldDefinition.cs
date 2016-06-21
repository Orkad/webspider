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
            return new Field(Name, value);
        }
    }
}