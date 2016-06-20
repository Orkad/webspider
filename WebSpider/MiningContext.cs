using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSpiderLib.Parsing;

namespace WebSpiderLib
{
    public class MiningContext
    {
        public WebExplorator Explorator;
        public WebExtractor Extractor;

        public event Action<WebPage> Explore; 
        public event Action<WebPage, Data> Extract;

        public MiningContext(string url, string[] exploratorGetFilter, DataDefinition dataDefinition)
        {
            Explorator = new WebExplorator(url,exploratorGetFilter);
            Extractor = new WebExtractor(Explorator,dataDefinition);
            Explorator.PageLoaded += page => Explore?.Invoke(page);
            Extractor.SuccessParse += (page, data) => Extract?.Invoke(page, data);
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
