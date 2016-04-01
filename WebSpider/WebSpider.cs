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
            
            WriteLineLog("");
            WriteLineLog("--------------------------------------------------");
            WriteLineLog("----------------- Starting spider ----------------");
            WriteLineLog("--------------------------------------------------");
            WriteLineLog("");
            WriteLineLog(" ...Options...");
            WriteLineLog(" Link filter => " + Filter);
            WriteLineLog(" Try again on fail => " + TryAgainOnFail);
            WriteLineLog(" Time between each try => " + TimeBetweenEachTry + " ms");
            WriteLineLog(" .............");
            WriteLineLog("");
            Links.Enqueue(startUrl);
            int i = 0;
            while (!Links.Empty())
            {
                WriteLineLog(" WebSpider : " + i++ + " explored links " + Links.Count() + " more");
                Process(new Uri(Links.Dequeue()));
                Log.Flush();
            }
            WriteLineLog("");
            WriteLineLog("--------------------------------------------------");
            WriteLineLog("------------------ Ending spider -----------------");
            WriteLineLog("--------------------------------------------------");
            WriteLineLog("");
        }


        private void WriteLog(string message)
        {
            Log?.Write(message);
        }

        private void WriteLineLog(string message)
        {
            Log?.WriteLine(message);
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

        private static string LoadHtml2(WebRequest request)
        {
            using (var myWebClient = new WebClient())
            {
                myWebClient.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                return myWebClient.DownloadString(request.RequestUri);
            }
        }

        private void Process(Uri uri)
        {
            do
            {
                try
                {
                    HttpWebRequest request = WebRequest.CreateHttp(uri);
                    WriteLog(" Read   => ");
                    WriteLineLog(uri.AbsoluteUri);
                    string html = LoadHtml(request);
                    ExploreEvent?.Invoke(html);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var links = doc.DocumentNode.Descendants("a");
                    int startLinksCount = Links.Count();
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
                    int newLinks = Links.Count() - startLinksCount;
                    
                    WriteLog(" Ok ");
                    if (newLinks > 0)
                        WriteLineLog("+"+newLinks+" links");
                    else
                        WriteLineLog("");
                    WriteLineLog("");
                    return;
                }
                catch (Exception e)
                {
                    WriteLineLog(" Fail");
                }
            } while (TryAgainOnFail);
        }
    }
}
