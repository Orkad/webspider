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

    public class WebSpiderOld
    {

        #region Properties

        /// <summary>
        /// Liste des liens a parcourir
        /// </summary>
        public Explorator<Uri> Links { get; } = new Explorator<Uri>();

        /// <summary>
        /// Filtre a appliquer sur les liens
        /// </summary>
        public FilterDelegate Filter { get; set; }

        #endregion

        /// <summary>
        /// Evenement lancé avant l'exploration d'une page
        /// </summary>
        public event Action BeforeExplore;

        /// <summary>
        /// Evenement lancé après l'exploration d'une page
        /// </summary>
        public event Action AfterExplore;

        /// <summary>
        /// Evenement lancé pendant la lecture html avec le code Html passé en paramètre
        /// </summary>
        public event Action<string> OnHtmlExplore;

        /// <summary>
        /// Evenement lancée lors d'une erreur au cours de la lecture
        /// </summary>
        public event Action<string> ErrorEvent;


        public void Run(Uri uri)
        {
            Links.Add(uri);
            while (!Links.Empty())
            {
                BeforeExplore?.Invoke();
                Process(Links.Explore());
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

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    OnHtmlExplore(html);

                    var links = doc.DocumentNode.Descendants("a");

                    foreach (var link in links)
                    {
                        var href = link.Attributes["href"]?.Value;
                        if (href == null)
                            continue;
                        if (href.StartsWith("/")) // Cas Url racine
                            href = "http://" + request.Host + href;

                    }
                    return;
                }
                catch (Exception e)
                {
                    ErrorEvent?.Invoke(e.Message);
                }
            } while (true);
        }
    }
}
