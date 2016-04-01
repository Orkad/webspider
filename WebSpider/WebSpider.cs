using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;

namespace WebSpiderLib
{
    public class WebSpider
    {
        /// <summary>
        /// Liste des liens a parcourir
        /// </summary>
        private UniqueQueue<string> Links { get; set; } = new UniqueQueue<string>(true);

        /// <summary>
        /// Filtre a appliquer sur les liens
        /// </summary>
        public string Filter { get; set; } = "";

        /// <summary>
        /// Le stream utilisé pour le log
        /// </summary>
        public TextWriter Log { get; set; }

        /// <summary>
        /// Réésayer a l'echec
        /// </summary>
        public bool TryAgainOnFail = true;

        /// <summary>
        /// Temps entre chaques requettes (ms)
        /// </summary>
        public int TimeBetweenEachTry = 0;

        /// <summary>
        /// 
        /// </summary>
        public event Action<string> ExploreEvent;

        public void Run(string startUrl)
        {
            WriteLog("");
            WriteLog("--------------------------------------------------");
            WriteLog("----------------- Starting spider ----------------");
            WriteLog("--------------------------------------------------");
            WriteLog("");
            WriteLog(" ...Options...");
            WriteLog(" Link filter => " + Filter);
            WriteLog(" Try again on fail => " + TryAgainOnFail);
            WriteLog(" Time between each try => " + TimeBetweenEachTry + " ms");
            WriteLog(" .............");
            WriteLog("");
            Links.Enqueue(startUrl);
            while (!Links.Empty())
            {
                Process(new Uri(Links.Dequeue()));
            }
            WriteLog("");
            WriteLog("--------------------------------------------------");
            WriteLog("------------------ Ending spider -----------------");
            WriteLog("--------------------------------------------------");
            WriteLog("");
        }

        private static string XpathSearch(HtmlDocument doc, string xpath) 
        {
            if (string.IsNullOrEmpty(xpath) || doc.DocumentNode.SelectSingleNode(xpath) == null)
                return null;
            return doc.DocumentNode.SelectSingleNode(xpath).InnerText;
        }

        private void WriteLog(string message)
        {
            Log?.WriteLine(message);
        }

        private void Process(Uri uri)
        {
            do
            {
                try
                {
                    Thread.Sleep(TimeBetweenEachTry);
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception();

                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = response.CharacterSet == null
                        ? new StreamReader(receiveStream)
                        : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    string html = readStream.ReadToEnd();
                    WriteLog(" Read => " + uri.AbsoluteUri);

                    if(ExploreEvent != null)
                        ExploreEvent(html);

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

                        if (href.Contains(Filter))
                            Links.Enqueue(href);
                    }

                    response.Close();
                    readStream.Close();
                    return;
                }
                catch (Exception e)
                {
                    WriteLog(" Fail => " + uri.AbsoluteUri);
                }
            } while (TryAgainOnFail);
        }
    }
}
