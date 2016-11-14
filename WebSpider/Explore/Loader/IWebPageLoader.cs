using System;

namespace WebSpiderLib.Explore.Loader
{
    public interface IWebPageLoader
    {
        event Action<WebPage> LoadSuccess;
        event Action<Uri> LoadError;
        void Load(Uri uri);
    }
}