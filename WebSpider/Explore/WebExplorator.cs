using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSpiderLib.Explore
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
    public class WebExplorator
    {
        /// <summary>
        /// HashSet assurant l'unicité des liens parcouru
        /// </summary>
        private readonly HashSet<string> _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Queue<Uri> _uriBuffer = new Queue<Uri>(); 
        private readonly List<Uri> _loadingPages = new List<Uri>();
        private readonly List<Uri> _errorPages = new List<Uri>();
        private readonly List<Uri> _successPages = new List<Uri>();
        private readonly string[] _explorationGetFilter;
        private readonly string _startUrl;
        private int MaxRequest = 5;


        public event Action<WebPageOld> PageLoaded;
        public event Action<WebPageOld> PageError;
        public event Action<Uri> PageFound;
        public int ExploredPages => _successPages.Count();
        public int LoadingPages => _loadingPages.Count();
        public bool Active => _loadingPages.Count() != 0;

        public WebExplorator(string startUrl, string[] explorationGetFilter)
        {
            _startUrl = startUrl;
            _explorationGetFilter = explorationGetFilter;
        }

        public void Start()
        {
            Explore(new Uri(_startUrl));
        }

        /// <summary>
        ///     Fonction d'exploration d'une page web (RECURSIF & ASYNC)
        ///         Condition du traitement d'une page : 
        ///          -inexplorée par l'instance courante
        ///          -respect du filtre
        ///          -Absolue
        /// </summary>
        /// <param name="uri"></param>
        private void Explore(Uri uri)
        {
            //Condition du traitement d'une page : 
            //inexplorée par l'instance courante
            //respect du filtre
            //Absolue
            if (!_hashSet.Add(uri.AbsoluteUri) || !ExplorationFilter(uri) || !uri.IsAbsoluteUri)
                return;
            PageFound?.Invoke(uri);
            if (LoadingPages > MaxRequest)
                FillBuffer(uri);
            else
                WebRequest(uri);
        }

        private bool ExplorationFilter(Uri uri)
        {
            string href = uri.AbsoluteUri;
            var getParam = uri.ParseQueryString();
            if (!href.Contains('#') && href.Contains(_startUrl))
            {
                if (getParam.Count != 0)
                {
                    foreach (var param in getParam)
                    {
                        foreach (var filter in _explorationGetFilter)
                        {
                            if (param.Key == filter)
                                return true;
                        }
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private void FillBuffer(Uri uri)
        {
            _uriBuffer.Enqueue(uri);
        }

        private void WebRequest(Uri uri)
        {
            _loadingPages.Add(new WebPageOld(uri, Callback).Adress);
        }

        /// <summary>
        ///     Callback de la fonction d'exploration
        /// </summary>
        /// <param name="webPage">la page reçue</param>
        private void Callback(WebPageOld webPage)
        {
            //Si la page est en erreur elle est alors mise dans la liste des pages d'erreurs
            if (webPage.CurrentState == WebPageOld.State.Error)
            {
                PageError?.Invoke(webPage);
                ((List<Uri>)_errorPages).Add(webPage.Adress);
            }
                
                
            else if (webPage.CurrentState == WebPageOld.State.Loaded)
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
