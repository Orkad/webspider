using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using System.Net;
using System.Net.Http;

namespace Orkad.WebSpider
{
    public class WebSpider
    {

        /// <summary>
        /// Filtre a appliquer sur les liens
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Le stream utilisé pour le log
        /// </summary>
        public TextWriter Log { get; set; }

        private HashSet<string> SkippedLinks { get; set; }

        private HashSet<string> Links { get; set; }

        public WebSpider(string filter, string startUrl, TextWriter log = null)
        {
            Log = log;
            Filter = filter;
            SkippedLinks = new HashSet<string>();
            Links = new HashSet<string>();


            Process(startUrl);

            
        }

        private void WriteLog(string message)
        {
            Log?.WriteLine(message);
        }

        public void Process(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Threading.Thread.Sleep(1000);
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                    WriteLog("Webspider : Success at " + url);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(readStream.ReadToEnd());
                    SkippedLinks.Add(url);

                    var links = doc.DocumentNode.Descendants("a");
                    foreach (var link in links)
                    {
                        var href = link.Attributes["href"]?.Value;
                        if (href == null)
                            continue;
                        if (href.StartsWith("/")) // Cas Url racine
                            href = "http://" + request.Host + href;


                        if (href != null && href.Contains(Filter) && !SkippedLinks.Contains(href))
                        {
                            WriteLog("Webspider : add link to explore => " + href);
                            Links.Add(href);
                        }
                    }

                    response.Close();
                    readStream.Close();
                }
                else
                    throw new Exception();
            }
            catch(Exception e)
            {
                WriteLog("Webspider : Can't read at => " + url);
                SkippedLinks.Add(url);
            }
        }
    }
}
