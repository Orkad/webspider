using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSpiderLib.Explore
{
    /// <summary>
    /// 
    /// </summary>
    public class WebExplorator
    {
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
        /// Uri de départ
        /// </summary>
        private readonly Uri _startUri;

        /// <summary>
        /// Nombre de requette maximales
        /// </summary>
        private int MaxRequest = 1;

        private readonly Func<Uri, bool> _uriValidator;



        public event Action<WebPage> PageLoaded;
        public event Action<Uri> PageError; 
        public event Action<Uri> PageFound;
        public event Action Done; 

        /// <summary>
        /// Constructeur d'un explorateur web
        /// </summary>
        /// <param name="startUri">Url de départ</param>
        /// <param name="uriValidator">Fonction définissant si une uri est valide ou non pour l'exploration</param>
        public WebExplorator(Uri startUri, Func<Uri, bool> uriValidator)
        {
            _startUri = startUri;
            _uriValidator = uriValidator;
        }

        /// <summary>
        /// Démarrage de l'explorateur web (explore la première page, en trouve d'autres respectant le filtre, les explore de la meme manière recursivement)
        /// </summary>
        public void Start()
        {
            Explore(_startUri);
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
            if (!_uriSet.Add(uri) || !_uriValidator(uri) || !uri.IsAbsoluteUri)
                return;
            PageFound?.Invoke(uri);
            if (_loadingPages.Count > MaxRequest)
                FillBuffer(uri);
            else
                WebRequest(uri);
        }

        /// <summary>
        ///     Fonction de remplissage du buffer
        /// </summary>
        /// <param name="uri"></param>
        private void FillBuffer(Uri uri)
        {
            _uriBuffer.Enqueue(uri);
        }

        /// <summary>
        ///     Genere une requette web asynchrone 
        /// </summary>
        /// <param name="uri"></param>
        private void WebRequest(Uri uri)
        {
            WebPageLoader.Load(uri, LoadSuccess, LoadError);
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
