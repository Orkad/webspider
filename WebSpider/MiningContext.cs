using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSpiderLib.Explore;
using WebSpiderLib.Extract;

namespace WebSpiderLib
{
    public class MiningContext
    {
        public WebExplorator Explorator;
        public WebExtractor Extractor;

        public event Action<WebPageOld> Explore; 
        public event Action<Data> Extract;

        public MiningContext(string url, string[] exploratorGetFilter, DataDefinition dataDefinition)
        {
            Explorator = new WebExplorator(url,exploratorGetFilter);
            Extractor = new WebExtractor(dataDefinition);
            Explorator.PageLoaded += page => {
                Explore?.Invoke(page);
                Extractor.Extract(page.Html);
            };
            Extractor.SuccessParse += (data) => Extract?.Invoke(data);
        }

        public void Run()
        {
            Explorator.Start();
        }

        public void SaveDataMining(string path)
        {
            Extractor.SaveDataBinary(path);
        }
    }
}
