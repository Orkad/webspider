using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    public delegate bool FilterDelegate(string href);

    public class WebSpider
    {
        /// <summary>
        /// Liste des liens a parcourir
        /// </summary>
        public UniqueQueue<string> Links { get; } = new UniqueQueue<string>(true);

        /// <summary>
        /// Réésayer a l'echec
        /// </summary>
        public bool TryAgainOnFail = true;

        /// <summary>
        /// Evenement déclenché au chargement d'une page html par le spider
        /// </summary>
        public event Action<string> HtmlEvent;

        /// <summary>
        /// Evenement déclenché au changement d'une adresse par le spider
        /// </summary>
        public event Action<Uri> CurrentUriChange;

        public event Action BeforeExplore;

        public event Action AfterExplore;

        /// <summary>
        /// Filtre a appliquer sur les liens
        /// </summary>
        public FilterDelegate Filter;

        public void Run(string startUrl)
        {
            Links.Enqueue(startUrl);
            int i = 0;
            while (!Links.Empty())
            {
                Uri uri = new Uri(Links.Dequeue());
                DateTime startTime = DateTime.Now;
                BeforeExplore?.Invoke();
                CurrentUriChange?.Invoke(uri);
                Process(uri);
                AfterExplore?.Invoke();
            }
        }

        private static string LoadHtml(WebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = response.CharacterSet == null
                ? new StreamReader(receiveStream)
                : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            string html = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return html;
        }

        private void Process(Uri uri)
        {
            do
            {
                try
                {
                    HttpWebRequest request = WebRequest.CreateHttp(uri);
                    string html = LoadHtml(request);

                    HtmlEvent?.Invoke(html);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var links = doc.DocumentNode.Descendants("a");

                    foreach (var link in links)
                    {
                        var href = link.Attributes["href"]?.Value;
                        if (href == null)
                            continue;
                        if (href.StartsWith("/")) // Cas Url racine
                            href = "http://" + request.Host + href;

                        if (Filter(href))
                            Links.Enqueue(href);

                    }

                    return;
                }
                catch (Exception e)
                {
                    
                }
            } while (TryAgainOnFail);
        }
    }
}
