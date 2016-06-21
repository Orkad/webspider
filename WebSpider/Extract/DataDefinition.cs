using System.Collections.Generic;
using HtmlAgilityPack;

namespace WebSpiderLib.Extract
{
    public class DataDefinition
    {
        public readonly List<FieldDefinition> FieldDefinition = new List<FieldDefinition>();

        public void AddXPathMatching(string paramName, string xpathRequest)
        {
            FieldDefinition.Add(new FieldDefinition(paramName, xpathRequest));
        }

        public Data Parse(HtmlDocument document)
        {
            Data data = new Data();
            foreach (var fieldDef in FieldDefinition)
            {
                var parsedField = fieldDef.Parse(document);
                if (parsedField == null)
                    return null;
                data.Fields.Add(parsedField);
            }
            return data;
        }
    }
}