using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib
{
    /// <summary>
    /// Définition d'un type de fonction pour un filtre
    /// </summary>
    /// <param name="uri">Adresse a filtrer</param>
    /// <returns></returns>
    public delegate bool FilterDelegate(Uri uri);

    /// <summary>
    /// 
    /// </summary>
    public class WebCrawler
    {
        /// <summary>
        /// HashSet assurant l'unicité des liens parcouru
        /// </summary>
        private readonly HashSet<string> _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly IEnumerable<WebPage> _loadingPages = new List<WebPage>();
        private readonly IEnumerable<WebPage> _errorPages = new List<WebPage>();
        private readonly IEnumerable<WebPage> _successPages = new List<WebPage>();
        private readonly FilterDelegate ExploreFilter;


        public Action<WebPage> OnPageLoaded;
        public Action<WebPage> OnPageError;
        public Action<Uri> OnPageFound;
        public int ExploredPages => _successPages.Count();
        public int LoadingPages => _loadingPages.Count();
        public bool Active => _loadingPages.Count() != 0;

        public WebCrawler(FilterDelegate exploreFilter)
        {
            ExploreFilter = exploreFilter;
        }

        /// <summary>
        ///     Fonction d'exploration d'une page web (RECURSIF & ASYNC)
        ///         Condition du traitement d'une page : 
        ///          -inexplorée par l'instance courante
        ///          -respect du filtre
        ///          -Absolue
        /// </summary>
        /// <param name="uri"></param>
        public void Explore(Uri uri)
        {
            //Condition du traitement d'une page : 
            //inexplorée par l'instance courante
            //respect du filtre
            //Absolue
            if (!_hashSet.Add(uri.AbsoluteUri) || !ExploreFilter(uri) || !uri.IsAbsoluteUri)
                return;
            OnPageFound?.Invoke(uri);
            ((List<WebPage>) _loadingPages).Add(new WebPage(uri, Callback));
        }

        /// <summary>
        ///     Callback de la fonction d'exploration
        /// </summary>
        /// <param name="webPage">la page reçue</param>
        private void Callback(WebPage webPage)
        {
            //Si la page est en erreur elle est alors mise dans la liste des pages d'erreurs
            if (webPage.CurrentState == WebPage.State.Error)
            {
                OnPageError?.Invoke(webPage);
                ((List<WebPage>)_errorPages).Add(webPage);
            }
                
                
            else if (webPage.CurrentState == WebPage.State.Loaded)
            {
                OnPageLoaded?.Invoke(webPage);
                ((List<WebPage>)_successPages).Add(webPage);
                foreach (var uri in webPage.Links)
                    Explore(uri);
            }
            else
                throw new Exception("WebCrawler Fatal error");
            ((List<WebPage>)_loadingPages).Remove(webPage);
        }
    }

}
