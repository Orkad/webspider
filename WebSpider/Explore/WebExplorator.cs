using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSpiderLib.Explore.Loader;

namespace WebSpiderLib.Explore
{
    /// <summary>
    /// 
    /// </summary>
    public class WebExplorator<T> where T : IWebPageLoader, new()
    {
        public event Action<WebPage> PageLoaded;
        public event Action<Uri> PageError;
        public event Action<Uri> PageFound;
        public event Action Done;
        public int remainingPages => _loadingPages.Count;
        public int bufferSize => _uriBuffer.Count;

        public readonly T Loader = new T();

        /// <summary>
        /// HashSet assurant l'unicité des liens parcouru
        /// </summary>
        private readonly HashSet<Uri> _uriSet = new HashSet<Uri>();

        /// <summary>
        /// Buffer stockant les uri en attente de traitement
        /// </summary>
        private readonly Queue<Uri> _uriBuffer = new Queue<Uri>();

        /// <summary>
        /// Pages en attente d'une réponse
        /// </summary>
        private readonly List<Uri> _loadingPages = new List<Uri>(); 

        /// <summary>
        /// Nombre de requette maximales
        /// </summary>
        private int MaxRequest = 15; // 5 - 50 for good performances

        private readonly Func<Uri, bool> _uriValidator;

        

        /// <summary>
        /// Constructeur d'un explorateur web
        /// </summary>
        /// <param name="startUri">Url de départ</param>
        /// <param name="uriValidator">Fonction définissant si une uri est valide ou non pour l'exploration</param>
        public WebExplorator(Func<Uri, bool> uriValidator)
        {
            _uriValidator = uriValidator;
            Loader.LoadSuccess += LoadSuccess;
            Loader.LoadError += LoadError;
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
            if (!_uriSet.Add(uri) || !_uriValidator(uri) || !uri.IsAbsoluteUri)
                return;
            PageFound?.Invoke(uri);
            if (_loadingPages.Count > MaxRequest)
                _uriBuffer.Enqueue(uri);
            else
                WebRequest(uri);
        }

        public void Explore(List<Uri> uris)
        {
            if (!uris.Any())
                return;
            Explore(uris[0]);
            foreach (var uri in uris)
                _uriBuffer.Enqueue(uri);
        }

        /// <summary>
        ///     Genere une requette web asynchrone 
        /// </summary>
        /// <param name="uri"></param>
        private void WebRequest(Uri uri)
        {
            Loader.Load(uri);
            _loadingPages.Add(uri);
        }

        private void LoadSuccess(WebPage webPage)
        {
            PageLoaded?.Invoke(webPage);
            foreach (var uri in webPage.Links)
                Explore(uri);
            _loadingPages.Remove(webPage.Adress);
            if (_uriBuffer.Count != 0)
                WebRequest(_uriBuffer.Dequeue());
            CheckDone();
        }

        private void LoadError(Uri uri)
        {
            _loadingPages.Remove(uri);
            PageError?.Invoke(uri);
            CheckDone();
        }

        private void CheckDone()
        {
            if (_loadingPages.Count == 0 && _uriBuffer.Count == 0)
                Done?.Invoke();
        }
    }
}
