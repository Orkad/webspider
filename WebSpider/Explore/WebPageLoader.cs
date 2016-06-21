using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using HtmlAgilityPack;

namespace WebSpiderLib.Explore
{ // The RequestState class passes data across async calls.


// ClientGetAsync issues the async request.
    public static class WebPageLoader
    {
        public class RequestState
        {
            public StringBuilder StrData;
            public byte[] BufferRead;
            public WebRequest Request;
            public Stream ResponseStream;
            public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
            public Action<WebPage> LoadCallback;

            public RequestState(WebRequest request, Action<WebPage> loadCallback)
            {
                Request = request;
                LoadCallback = loadCallback;
                BufferRead = new byte[BUFFER_SIZE];
                StrData = new StringBuilder(String.Empty);
                Request = null;
                ResponseStream = null;
            }
        }

        public const int BUFFER_SIZE = 1024;

        public static void Load(Uri uri, Action<WebPage> callback)
        {
            
            // Création de la requète
            WebRequest wreq = WebRequest.Create(uri);
            // Création de l'état de la requète
            RequestState rs = new RequestState(wreq, callback);
            // Début de la requète asynchrone
            wreq.BeginGetResponse(RespCallback, rs);
        }

        private static void RespCallback(IAsyncResult ar)
        {
            // Récupération de l'état de la requète
            RequestState rs = (RequestState) ar.AsyncState;
            // Récupération de la requète
            WebRequest req = rs.Request;

            // Call EndGetResponse, which produces the WebResponse object
            //  that came from the request issued above.
            WebResponse resp = req.EndGetResponse(ar);

            //  Start reading data from the response stream.
            Stream ResponseStream = resp.GetResponseStream();

            // Store the response stream in RequestState to read 
            // the stream asynchronously.
            rs.ResponseStream = ResponseStream;

            //  Pass rs.BufferRead to BeginRead. Read data into rs.BufferRead
            IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE,ReadCallBack, rs);
        }


        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            // Get the RequestState object from AsyncResult.
            RequestState rs = (RequestState) asyncResult.AsyncState;

            // Retrieve the ResponseStream that was set in RespCallback. 
            Stream responseStream = rs.ResponseStream;

            // Read rs.BufferRead to verify that it contains data. 
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                // Append the recently read data to the RequestData stringbuilder
                // object contained in RequestState.
                rs.StrData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));
                // Continue reading data until 
                // responseStream.EndRead returns –1.
                responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE,ReadCallBack, rs);
            }
            else
            {
                string html = rs.StrData.ToString();
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
                            uri = new Uri(rs.Request.RequestUri, uri);
                        links.Add(uri);
                    }
                }
                // Close down the response stream.
                responseStream.Close();

                rs.LoadCallback(new WebPage(rs.Request.RequestUri,html,links));
                
            }
        }
    }
}