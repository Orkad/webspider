using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace WebSpiderLib.Explore
{
    public class WebPageOld
    {
        /// <summary>
        /// Web page state
        /// </summary>
        public enum State { Loading, Error, Loaded}

        /// <summary>
        /// Adresse de la page
        /// </summary>
        public Uri Adress { get; }

        /// <summary>
        /// Liens présent sur la page
        /// </summary>
        public IEnumerable<Uri> Links { get; }

        /// <summary>
        /// Retour dans le cas ou le chargement s'est bien passé
        /// </summary>
        private Action<WebPageOld> CallBack { get; }

        /// <summary>
        /// Code Html de la page
        /// </summary>
        public string Html { get; private set; }

        /// <summary>
        /// Etat de la page web
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Charge une page web
        /// </summary>
        /// <param name="uri"></param>
        public WebPageOld(Uri uri, Action<WebPageOld> callback)
        {
            Adress = uri;
            CurrentState = State.Loading;
            Html = string.Empty;
            CallBack = callback;
            Links = new List<Uri>();
            WebRequest request = WebRequest.CreateHttp(uri);
            request.BeginGetResponse(OnLoaded, request);
        }

        /// <summary>
        /// Ajoute un lien a la page web
        /// </summary>
        /// <param name="uri">lien trouvé sur la page</param>
        private void AddLink(Uri uri)
        {
            ((List<Uri>) Links).Add(uri);
        }

        /// <summary>
        /// Fonction appellé lors du succès de la requête
        /// </summary>
        /// <param name="ar"></param>
        private void OnLoaded(IAsyncResult ar)
        {
            lock (this)
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);
                    Stream responseStream = response.GetResponseStream();

                    using (var reader = new StreamReader(responseStream))
                        Html = reader.ReadToEnd();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(Html);

                    var links = doc.DocumentNode.Descendants("a");

                    foreach (var link in links)
                    {
                        string href = link.Attributes["href"]?.Value;
                        if (href != null)
                        {
                            Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
                            if (!uri.IsAbsoluteUri)
                                uri = new Uri(request.RequestUri, uri);
                            AddLink(uri);
                        }
                    }
                    CurrentState = State.Loaded;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(request.Address);
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    CurrentState = State.Error;
                }
                CallBack.Invoke(this);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var stream = (Stream)ar.AsyncState;
            stream.EndRead(ar);


        }

        public static bool operator ==(WebPageOld wp1, WebPageOld wp2)
        {
            return wp1.Adress.AbsoluteUri == wp2.Adress.AbsoluteUri;
        }

        public static bool operator !=(WebPageOld wp1, WebPageOld wp2)
        {
            return !(wp1 == wp2);
        }
    }
}
