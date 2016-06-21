using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebSpiderLib.Explore
{
    public class HttpPageLoader
    {
        public class RequestState
        {
            public WebRequest Request;
            public Action<WebPage> LoadCallback;
            public Stream ResponseStream;
            

            public RequestState(WebRequest request, Action<WebPage> loadCallback)
            {
                Request = request;
                LoadCallback = loadCallback;
            }
        }
        private readonly Action<WebPage> _loaded;
         
        public static void Load(Uri uri, Action<WebPage> callback)
        {
            //_loaded = callback;
            // Create a new HttpWebRequest object.
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            // start the asynchronous operation
            request.BeginGetRequestStream(GetRequestStreamCallback, request);
        }

        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            Console.WriteLine("Please enter the input data to be posted:");
            string postData = Console.ReadLine();

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write to the request stream.
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(GetResponseCallback, request);
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            string html = streamRead.ReadToEnd();

            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.Descendants("a");
            var links = new List<Uri>();

            foreach (var link in nodes)
            {
                string href = link.Attributes["href"]?.Value;
                if (href != null)
                {
                    Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri)
                        uri = new Uri(request.RequestUri, uri);
                    links.Add(uri);
                }
            }

            // Release the HttpWebResponse
            response.Close();

            //_loaded?.Invoke(new WebPage(request.RequestUri, html, links));
        }
    }
}
