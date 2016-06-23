using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace WebSpiderLib.Explore
{
    public class WebPage
    {
        /// <summary>
        /// Adresse de la page
        /// </summary>
        public Uri Adress { get; }

        /// <summary>
        /// Code Html de la page
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Liens présent sur la page
        /// </summary>
        public List<Uri> Links { get; } = new List<Uri>();

        /// <summary>
        /// Constructeur d'une page web (la requete web dois etre effectuée avant)
        /// </summary>
        /// <param name="adress"></param>
        /// <param name="html"></param>
        /// <param name="links"></param>
        internal WebPage(Uri adress, string html)
        {
            Adress = adress;
            Html = html;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.Descendants("a");

            foreach (var link in nodes)
            {
                string href = link.Attributes["href"]?.Value;
                if (href != null)
                {
                    Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                        uri = new Uri(adress, uri);
                    Links.Add(uri);
                }
            }
        }
    }
}
