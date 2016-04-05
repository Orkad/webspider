using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    /// <summary>
    /// Définition d'un type de fonction pour un filtre
    /// </summary>
    /// <param name="uri">Adresse a filtrer</param>
    /// <returns></returns>
    public delegate bool FilterDelegate(Uri uri);

    public class WebSpider
    {

        /// <summary>
        /// Liste des liens a parcourir
        /// </summary>
        private Explorator<Uri> Links { get; } = new Explorator<Uri>();

        private bool Running { get; set; }

        /// <summary>
        /// Nombre de requettes totales en cours
        /// </summary>
        public int RequestCount { get; set; }

        public int NumberOfTryOnError { get; set; } = 3;

        /// <summary>
        /// Nombre de réponses totales
        /// </summary>
        public int ResponseCount { get; set; }

        /// <summary>
        /// Temps passé depuis le lancement du spider
        /// </summary>
        public System.Timers.Timer TimerFromStart { get; } = new System.Timers.Timer();

        /// <summary>
        /// Thread principal
        /// </summary>
        private Thread mainThread;

        /// <summary>
        /// Filtre des liens
        /// </summary>
        public FilterDelegate Filter = href => true;

        /// <summary>
        /// Evenement de log
        /// </summary>
        public event Action<string> Log;

        /// <summary>
        /// Evenement se déclenchant sur chaques pages html (URI, HTML string)
        /// </summary>
        public event Action<Uri,string> HtmlAction;

        private void Init()
        {
            Links.Clear();
            RequestCount = 0;
            ResponseCount = 0;
        }

        /// <summary>
        /// Démarrage du spider
        /// </summary>
        /// <param name="url"></param>
        public void Start(string url)
        {
            Links.Clear();
            try
            {
                Uri startUri = new Uri(url);
                if (!startUri.IsAbsoluteUri)
                    throw new WebSpiderException("L'adresse de départ n'est pas absolue");
                Links.Add(startUri);
                Running = true;
                mainThread = new Thread(Process);
                mainThread.Start();
                TimerFromStart.Start();
                Log?.Invoke("Démarrage du WebSpider");
            }
            catch (UriFormatException)
            {
                throw new WebSpiderException("L'adresse de départ est invalide");
            }
        }

        public void Stop()
        {
            mainThread?.Abort();
            mainThread = null;
            Running = false;
            TimerFromStart.Stop();
            Log?.Invoke("Arrêt du WebSpider");
        }

        public void Continue()
        {
            Running = true;
            TimerFromStart.Start();
            Log?.Invoke("Reprise du WebSpider");
        }

        public void Pause()
        {
            Running = false;
            TimerFromStart.Stop();
            Log?.Invoke("Pause du WebSpider");
        }

        /// <summary>
        /// Boucle principale du spider
        /// </summary>
        private void Process()
        {
            while (!Links.Empty() || RequestCount != 0)
            {
                if (Running)
                {
                    if (!Links.Empty())
                        ExploreUri(Links.Explore());
                }   
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
                Log?.Invoke("Erreur WebSpider : " + e.Message);
            }
        }

        /// <summary>
        /// Fonction asynchrone se déclenchant lors de la réponse d'une requette web
        /// </summary>
        /// <param name="result">resultat asynchrone</param>
        private void OnUriExplored(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            if (mainThread == null)
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
                HtmlAction?.Invoke(request.RequestUri,html);

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
                Log?.Invoke("Reponse => " + request.RequestUri.AbsoluteUri);
                ResponseCount++;
            }
            catch (Exception e)
            {
                Log?.Invoke("Erreur WebSpider => " + request.RequestUri.AbsoluteUri);
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
