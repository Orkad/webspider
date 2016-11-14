using System;
using System.IO;
using System.Net;
using System.Web;

namespace WebSpiderLib.Explore.Loader
{
    public class WebPageLoaderSync : IWebPageLoader
    {

        public event Action<WebPage> LoadSuccess;
        public event Action<Uri> LoadError;

        public void Load(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            try
            {
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    LoadSuccess(new WebPage(request.RequestUri, HttpUtility.HtmlDecode(reader.ReadToEnd())));
            }
            catch (Exception)
            {
                LoadError(request.RequestUri);
            }
        }
    }
}