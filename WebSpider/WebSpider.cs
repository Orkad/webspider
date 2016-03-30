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

        private Stack<string> links { get; set; }

        public WebSpider(string filter, string startUrl, TextWriter log = null)
        {
            Log = log;
            Process(startUrl);
        }

        private void WriteLog(string message)
        {
            Log?.WriteLine(message);
        }

        public void Process(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                WriteLog("Webspider : Success at " + url);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(readStream.ReadToEnd());
                FindLink(doc, "");

                response.Close();
                readStream.Close();
            }
            else
                WriteLog("Webspider : Can't read at => " + url);
        }

        public void FindLink(HtmlDocument doc, string linkFilter)
        {
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a/@href");
            foreach(var node in nodes)
            {
                WriteLog("Webspider : Link found => " + node.OuterHtml);
            }
        }
    }
}
