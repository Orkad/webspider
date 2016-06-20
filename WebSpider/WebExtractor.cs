using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib
{
    public class WebExtractor
    {
        private readonly XPathDataDefinition DataDefinition;

        public readonly List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();

        public event Action<WebPage,Dictionary<string,string>> SuccessParse;
        public event Action<WebPage> FailParse;  

        public WebExtractor(WebExplorer explorer, XPathDataDefinition dataDefinition)
        {
            explorer.PageLoaded += OnPageLoaded;
            DataDefinition = dataDefinition;
        }

        private void OnPageLoaded(WebPage webPage)
        {
            try
            {
                var data = DataDefinition.ParseWebPage(webPage.Html);
                Data.Add(data);
                SuccessParse?.Invoke(webPage, data);
            }
            catch (Exception)
            {
                FailParse?.Invoke(webPage);
            }
        }
    }
}
