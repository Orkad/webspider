using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HtmlAgilityPack;

namespace WebSpiderLib.Parsing
{
    public class WebExtractor
    {
        private readonly DataDefinition DataDefinition;

        public readonly List<Data> Data = new List<Data>();

        public event Action<WebPage,Data> SuccessParse;
        public event Action<WebPage> FailParse;  

        public WebExtractor(WebExplorator explorer, DataDefinition dataDefinition)
        {
            explorer.PageLoaded += OnPageLoaded;
            DataDefinition = dataDefinition;
        }

        private void OnPageLoaded(WebPage webPage)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(webPage.Html);
                var data = DataDefinition.Parse(document);
                Data.Add(data);
                SuccessParse?.Invoke(webPage, data);
            }
            catch (Exception)
            {
                FailParse?.Invoke(webPage);
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
