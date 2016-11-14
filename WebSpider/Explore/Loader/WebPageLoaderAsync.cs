using System;
using System.IO;
using System.Net;
using System.Web;

namespace WebSpiderLib.Explore.Loader
{
    public class WebPageLoaderAsync : IWebPageLoader
    {

        public event Action<WebPage> LoadSuccess;
        public event Action<Uri> LoadError;

        public void Load(Uri uri)
        {
            // Création de la requète
            HttpWebRequest wreq = (HttpWebRequest)WebRequest.Create(uri);
            // Début de la requète asynchrone
            wreq.BeginGetResponse(RespCallback, wreq);
        }

        private void RespCallback(IAsyncResult ar)
        {
            // Récupération de l'état de la requète
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            try
            {
                //  Start reading data from the response stream.
                using (StreamReader reader = new StreamReader(((HttpWebResponse)request.EndGetResponse(ar)).GetResponseStream()))
                    LoadSuccess(new WebPage(request.RequestUri, HttpUtility.HtmlDecode(reader.ReadToEnd())));
            }
            catch (Exception)
            {
                LoadError?.Invoke(request.RequestUri);
            }
            
        }
    }
}