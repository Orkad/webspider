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
    public class WebExplorer : IExplorer
    {
        /// <summary>
        /// HashSet assurant l'unicité des liens parcouru
        /// </summary>
        private readonly HashSet<string> _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Queue<Uri> _uriBuffer = new Queue<Uri>(); 
        private readonly List<Uri> _loadingPages = new List<Uri>();
        private readonly List<Uri> _errorPages = new List<Uri>();
        private readonly List<Uri> _successPages = new List<Uri>();
        private readonly FilterDelegate ExploreFilter;
        private int MaxRequest = 1;


        public event Action<WebPage> PageLoaded;
        public event Action<WebPage> PageError;
        public event Action<Uri> PageFound;
        public int ExploredPages => _successPages.Count();
        public int LoadingPages => _loadingPages.Count();
        public bool Active => _loadingPages.Count() != 0;

        public WebExplorer(FilterDelegate exploreFilter)
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
            PageFound?.Invoke(uri);
            if (LoadingPages > MaxRequest)
                FillBuffer(uri);
            else
                WebRequest(uri);
        }

        private void FillBuffer(Uri uri)
        {
            _uriBuffer.Enqueue(uri);
        }

        private void WebRequest(Uri uri)
        {
            _loadingPages.Add(new WebPage(uri, Callback).Adress);
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
                PageError?.Invoke(webPage);
                ((List<Uri>)_errorPages).Add(webPage.Adress);
            }
                
                
            else if (webPage.CurrentState == WebPage.State.Loaded)
            {
                PageLoaded?.Invoke(webPage);
                ((List<Uri>)_successPages).Add(webPage.Adress);
                foreach (var uri in webPage.Links)
                    Explore(uri);
            }
            else
                throw new Exception("WebCrawler Fatal error");
            _loadingPages.Remove(webPage.Adress);
            if (_uriBuffer.Count != 0)
                WebRequest(_uriBuffer.Dequeue());
        }
    }
}
