using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace WebSpiderLib.Explore
{
    internal class WebPage
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
        public List<Uri> Links { get; }

        /// <summary>
        /// Constructeur d'une page web (la requete web dois etre effectuée avant)
        /// </summary>
        /// <param name="adress"></param>
        /// <param name="html"></param>
        /// <param name="links"></param>
        internal WebPage(Uri adress, string html, List<Uri> links)
        {
            Adress = adress;
            Html = html;
            Links = links;
        }
    }
}
