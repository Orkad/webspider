using System;
using System.Collections.Generic;
using System.Web;

namespace WebSpiderLib.Explore
{
    public static class UriExt
    {
        public static IReadOnlyDictionary<string, List<string>> ParseQueryString(this Uri uri)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            var parameters = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(uri.Query));
            foreach (string key in parameters.AllKeys)
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(parameters[key]);
                }
                else
                {
                    dictionary.Add(key,new List<string> {parameters[key]});
                }
            }
            return dictionary;
        }
    }
}
