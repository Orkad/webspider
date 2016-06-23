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

        public event Action<WebPage> Explore;
        public event Action<Uri> ExploreError; 
        public event Action<Data> Extract;
        public event Action Done; 

        /// <summary>
        /// Constructeur d'un contexte de minage de données web
        /// </summary>
        /// <param name="startUri">Uri de départ de l'exploration</param>
        /// <param name="uriValidator">Fonction de validation des liens a parcourir</param>
        /// <param name="dataDefinition">Définition des données a extraire</param>
        public MiningContext(Uri startUri, Func<Uri,bool> uriValidator, DataDefinition dataDefinition)
        {
            Explorator = new WebExplorator(startUri, uriValidator);
            Extractor = new WebExtractor(dataDefinition);
            Explorator.PageLoaded += page => {
                Explore?.Invoke(page);
                Extractor.Extract(page.Html);
            };
            Explorator.PageError += uri => ExploreError?.Invoke(uri);
            Explorator.Done += () => Done?.Invoke();
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
