using System;
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
        private UniqueQueue<string> Links { get; set; }

        /// <summary>
        /// Filtre a appliquer sur les liens
        /// </summary>
        public string Filter { get; set; }

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
        public int TimeBetweenEachTry = 500;

        public WebSpider(string filter, string startUrl, TextWriter log = null)
        {
            Log = log;
            Filter = filter;
            WriteLog("");
            WriteLog("--------------------------------------------------");
            WriteLog("----------------- Starting spider ----------------");
            WriteLog("--------------------------------------------------");
            WriteLog("");
            WriteLog(" ...Options...");
            WriteLog(" Starting url => " + startUrl);
            WriteLog(" Link filter => " + Filter);
            WriteLog(" Try again on fail => " + TryAgainOnFail);
            WriteLog(" Time between each try => " + TimeBetweenEachTry + " ms");
            WriteLog(" .............");
            WriteLog("");
            Links = new UniqueQueue<string>(true);
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

        private void WriteLog(string message)
        {
            Log?.WriteLine(message);
        }

        public void Process(Uri uri)
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
