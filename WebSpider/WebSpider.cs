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
        /// Nombre de réponses totales
        /// </summary>
        public int ResponseCount { get; set; }

        /// <summary>
        /// Temps passé depuis le lancement du spider
        /// </summary>
        public System.Timers.Timer TimerFromStart { get; } = new System.Timers.Timer();

        /// <summary>
        /// Filtre des liens
        /// </summary>
        public FilterDelegate Filter = href => true;

        /// <summary>
        /// Log complet
        /// </summary>
        public volatile EventHandler<string> Log;

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public volatile EventHandler<string> ErrorLog;

        /// <summary>
        /// Evenement se déclenchant sur chaques pages html (URI, HTML string)
        /// </summary>
        public event Action<Uri,string> HtmlAction;

        static WebSpider()
        {
            WebRequest.DefaultWebProxy = null;
        }

        private void Init()
        {
            Links.Clear();
            RequestCount = 0;
            ResponseCount = 0;
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
            TimerFromStart.Start();
            Log?.AsyncSafeInvoke(this,"Démarrage du WebSpider");
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
            TimerFromStart.Stop();
            Log?.AsyncSafeInvoke(this,"Arrêt du WebSpider");
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
                ErrorLog?.AsyncSafeInvoke(this,"Erreur WebSpider : " + e.Message);
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
                Log?.Invoke(this,"Reponse => " + request.RequestUri.AbsoluteUri);
                ResponseCount++;
            }
            catch (Exception e)
            {
                ErrorLog?.AsyncSafeInvoke(this,"Erreur WebSpider => " + e.Message + " pour " + request.RequestUri.AbsoluteUri);
                Links.Add(request.RequestUri);
            }
            
            RequestCount--;
        }
    }

    public class WebSpiderException : Exception
    {
        public WebSpiderException(string message) : base("Erreur WebSpider : " + message) { }
    }

    public static class ThreadExtension
    {

        /// <summary>
        /// This method safely calls the each event handler attached to the event. This method uses <see cref="System.Threading.Tasks"/> to
        /// asynchronously call invoke without any exception handling. As such, if any of the event handlers throw exceptions the application will
        /// most likely crash when the task is collected. This is an explicit decision since it is really in the hands of the event handler
        /// creators to make sure they handle issues that occur do to their code. There isn't really a way for the event raiser to know
        /// what is going on.
        /// </summary>
        /// <param name="e">The event to call</param>
        /// <param name="sender">The sender of the event</param>
        /// <param name="arg">The argument for the event</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void AsyncSafeInvoke<T>(this EventHandler<T> e, object sender, T arg)
        {
            // Used to make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<T> handler = e;
            if (handler != null)
            {
                // Manually calling all event handlers so that we could capture and aggregate all the
                // exceptions that are thrown by any of the event handlers attached to this event.  
                var invocationList = handler.GetInvocationList();

                Task.Factory.StartNew(() =>
                {
                    foreach (var @delegate in invocationList)
                    {
                        var h = (EventHandler<T>) @delegate;
                        // Explicitly not catching any exceptions. While there are several possibilities for handling these 
                        // exceptions, such as a callback, the correct place to handle the exception is in the event handler.
                        h.Invoke(sender, arg);
                    }
                });
            }
        }
    }
}
