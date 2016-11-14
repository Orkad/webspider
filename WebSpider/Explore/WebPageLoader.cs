using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
            public HttpWebRequest Request;
            public Stream ResponseStream;
            public Decoder StreamDecode = Encoding.UTF8.GetDecoder();
            public Action<WebPage> LoadSuccess;
            public Action<Uri> LoadError;
            public int trycount;

            public RequestState(HttpWebRequest request, Action<WebPage> loadSuccess, Action<Uri> loadError)
            {
                Request = request;
                LoadSuccess = loadSuccess;
                LoadError = loadError;
                BufferRead = new byte[BUFFER_SIZE];
                StrData = new StringBuilder();
            }
        }

        public const int BUFFER_SIZE = 4096;
        public const int MaxTry = 2;

        public static async Task<WebPage> LoadASync(Uri uri)
        {
            try
            {
                HttpWebRequest webRequest = WebRequest.CreateHttp(uri);

                HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();

                StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());

                string html = await streamReader.ReadToEndAsync();

                return new WebPage(uri, HttpUtility.HtmlDecode(html));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static WebPage Load(Uri uri)
        {
            try
            {
                HttpWebRequest webRequest = WebRequest.CreateHttp(uri);

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());

                string html = streamReader.ReadToEnd();

                return new WebPage(uri, HttpUtility.HtmlDecode(html));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Load(Uri uri, Action<WebPage> loadSuccess, Action<Uri> loadError)
        {
            
            // Création de la requète
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(uri);
            // Création de l'état de la requète
            RequestState rs = new RequestState(wreq, loadSuccess, loadError);
            object _lock = new object();
            // Début de la requète asynchrone
            wreq.BeginGetResponse(new AsyncCallback(RespCallback), rs);
        }

        private static void RespCallback(IAsyncResult ar)
        {
            // Récupération de l'état de la requète
            RequestState rs = (RequestState)ar.AsyncState;
            try
            {
                
                // Récupération de la requète
                HttpWebRequest req = rs.Request;

                // Call EndGetResponse, which produces the WebResponse object
                //  that came from the request issued above.
                HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(ar);

                //  Start reading data from the response stream.
                Stream responseStream = resp.GetResponseStream();

                StreamReader reader = new StreamReader(responseStream);

                string html = reader.ReadToEnd();

                responseStream.Close();

                rs.LoadSuccess(new WebPage(rs.Request.RequestUri, HttpUtility.HtmlDecode(html)));
            }
            catch (Exception)
            {

                rs.LoadError?.Invoke(rs.Request.RequestUri);
            }
            
        }


        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            
            // Get the RequestState object from AsyncResult.
            RequestState rs = (RequestState) asyncResult.AsyncState;
           

            try
            {

                var responseStream = rs.ResponseStream;
                var read = responseStream.EndRead(asyncResult);
                if (read > 0)
                {
                    // Append the recently read data to the RequestData stringbuilder
                    // object contained in RequestState.
                    rs.StrData.Append(Encoding.UTF8.GetString(rs.BufferRead, 0, read));
                    // Continue reading data until 
                    // responseStream.EndRead returns –1.
                    responseStream.BeginRead(rs.BufferRead, 0, BUFFER_SIZE, ReadCallBack, rs);
                }
                else
                {
                    string html = rs.StrData.ToString();
                    responseStream.Close();
                    rs.LoadSuccess(new WebPage(rs.Request.RequestUri, HttpUtility.HtmlDecode(html)));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Erreur de lecture de l'uri : " + rs.Request.RequestUri);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
                System.Diagnostics.Debug.WriteLine(e.Message);
                rs.LoadError?.Invoke(rs.Request.RequestUri);
            }
        }
    }
}