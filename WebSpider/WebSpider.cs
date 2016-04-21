using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    

    public class WebSpider
    {
        /// <summary>
        /// Liste des liens a parcourir
        /// </summary>
        private Explorator<Uri> Links { get; } = new Explorator<Uri>();

        /// <summary>
        /// Thread principal
        /// </summary>
        private Thread _mainThread;

        /// <summary>
        /// Nombre de requêtes par secondes souhaité (0 pas de limite)
        /// </summary>
        public double Frequency { get; set; } = 0;

        /// <summary>
        /// Nombre de requettes totales en cours
        /// </summary>
        public int RequestCount { get; set; }

        /// <summary>
        /// Nombre d'essais sur une erreur
        /// </summary>
        public int NumberOfTryOnError { get; set; } = 3;

        /// <summary>
        /// Filtre des liens
        /// </summary>
        public FilterDelegate Filter = href => true;

        /// <summary>
        /// Log complet
        /// </summary>
        public EventHandler<string> Log;

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public EventHandler<string> ErrorLog;

        static WebSpider()
        {
            WebRequest.DefaultWebProxy = null;
        }

        private void Init()
        {
            Links.Clear();
            RequestCount = 0;
        }

        /// <summary>
        /// Démarrage du spider
        /// </summary>
        /// <param name="uri"></param>
        public void Start(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new WebSpiderException("L'adresse de départ n'est pas absolue");
            Links.Add(uri);
            _mainThread = new Thread(Process);
            _mainThread.Start();
            Log?.Invoke(this,"Démarrage du WebSpider");
        }

        /// <summary>
        /// Démarrage du spider
        /// </summary>
        /// <param name="url"></param>
        public void Start(string url)
        {
            Init();
            try
            {
                Start(new Uri(url));
            }
            catch (UriFormatException)
            {
                throw new WebSpiderException("L'adresse de départ est invalide");
            }
        }

        public void Stop()
        {
            _mainThread?.Abort();
            _mainThread = null;
            Log?.Invoke(this,"Arrêt du WebSpider");
        }

        /// <summary>
        /// Boucle principale du spider
        /// </summary>
        private void Process()
        {
            while (!Links.Empty() || RequestCount != 0)
            {
                if (!Links.Empty())
                    ExploreUri(Links.Explore());
                if(Frequency != 0.0)
                    Thread.Sleep((int)(1000 / Frequency));
            }
        }

        /// <summary>
        /// Fonction emetant une requette web qui sera receptionné par sa fonction asynchrone
        /// </summary>
        /// <param name="uri">adresse pour la requette</param>
        private void ExploreUri(Uri uri)
        {
            try
            {
                WebRequest request = WebRequest.CreateHttp(uri);
                request.BeginGetResponse(OnUriExplored, request);
                RequestCount++;
            }
            catch(Exception e)
            {
                ErrorLog?.Invoke(this,"Erreur WebSpider : " + e.Message);
            }
        }

        /// <summary>
        /// Fonction asynchrone se déclenchant lors de la réponse d'une requette web
        /// </summary>
        /// <param name="result">resultat asynchrone</param>
        private void OnUriExplored(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            
            if (_mainThread == null)
            {
                Links.Add(request.RequestUri);
                return;
            }
            try
            {
                HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(result);

                Stream streamResponse = response.GetResponseStream();
                StreamReader reader = new StreamReader(streamResponse);
                string html = reader.ReadToEnd();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var links = doc.DocumentNode.Descendants("a");
            
                foreach (var link in links)
                {
                    string href = link.Attributes["href"]?.Value;
                    if (href != null)
                    {
                        Uri uri = new Uri(href,UriKind.RelativeOrAbsolute);
                        if(!uri.IsAbsoluteUri)
                            uri = new Uri(request.RequestUri, uri);
                        if (Filter(uri))
                            Links.Add(uri);
                    }
                }
                Log?.Invoke(this,"Reponse => " + request.RequestUri.AbsoluteUri);
            }
            catch (Exception e)
            {
                Links.Add(request.RequestUri);
            }
            
            RequestCount--;
        }
    }

    public class WebSpiderException : Exception
    {
        public WebSpiderException(string message) : base("Erreur WebSpider : " + message) { }
    }
}
