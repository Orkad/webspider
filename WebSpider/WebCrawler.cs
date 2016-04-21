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

    public class WebCrawler
    {
        /// <summary>
        /// HashSet assurant l'unicité des liens parcouru
        /// </summary>
        private readonly HashSet<string> _hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly IEnumerable<WebPage> _loadingPages = new List<WebPage>();
        private readonly IEnumerable<WebPage> _errorPages = new List<WebPage>();
        private readonly IEnumerable<WebPage> _successPages = new List<WebPage>();
        public int ExploredPages => _successPages.Count();
        private readonly FilterDelegate Filter;
        private readonly Action<string> Log;
        private readonly Action<string> ErrorLog;
        public bool Run = true;

        public WebCrawler(FilterDelegate filter, Action<string> log, Action<string> errorLog)
        {
            Filter = filter;
            Log = log;
            ErrorLog = errorLog;
            Log?.BeginInvoke("WebCrawler instance created",null,this);
        }

        public void Explore(Uri uri)
        {
            if (!_hashSet.Add(uri.AbsoluteUri))
                ;
            else if (!Filter(uri))
                ErrorLog?.Invoke("Filter deny (" + uri.AbsoluteUri + ")");
            else if (!uri.IsAbsoluteUri)
                ErrorLog?.Invoke("Can't explore a relative uri (" + uri.AbsoluteUri + ")");
            else
            {
                ((List<WebPage>) _loadingPages).Add(new WebPage(uri, Callback));
                Log?.BeginInvoke("Request (" + uri.AbsoluteUri + ")",null,this);
            }
        }

        private void Callback(WebPage webPage)
        {

            if (webPage.CurrentState == WebPage.State.Error)
            {
                ((List<WebPage>)_errorPages).Add(webPage);
                ErrorLog?.BeginInvoke("Response (" + webPage.Adress.AbsoluteUri + ")",null,this);
            }
                
            else if (webPage.CurrentState == WebPage.State.Loaded)
            {
                Log?.BeginInvoke("Response (" + webPage.Adress.AbsoluteUri + ")",null,this);
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
