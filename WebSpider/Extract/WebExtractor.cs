using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace WebSpiderLib.Extract
{
    public class WebExtractor
    {
        private readonly DataDefinition DataDefinition;

        public readonly List<Data> Data = new List<Data>();

        public event Action<Data> SuccessParse;
        public event Action FailParse;
        public event Action ErrorParse;

        public WebExtractor(DataDefinition dataDefinition)
        {
            DataDefinition = dataDefinition;
        }

        public void Extract(string html)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(html);
                var data = DataDefinition.Parse(document);
                if (data != null)
                {
                    Data.Add(data);
                    SuccessParse?.Invoke(data);
                }
                else
                {
                    FailParse?.Invoke();
                }
            }
            catch (Exception)
            {
                ErrorParse?.Invoke();
            }
        }

        public void SaveDataXml(string path)
        {
            Serializer.SaveXml(path,Data);
        }

        public void SaveDataBinary(string path)
        {
            Serializer.Save(path,Data);
        }
    }
}
