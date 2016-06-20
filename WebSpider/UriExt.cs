using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebSpiderLib
{
    public static class UriExt
    {
        public static IReadOnlyDictionary<string, List<string>> ParseQueryString(this Uri uri)
        {
            var dict = new Dictionary<string, List<string>>();
            if (uri.Query != string.Empty)
                foreach (var param in uri.Query.Remove(0,1).Split('&'))
                {
                    string[] chain = param.Split('=');
                    if(dict.ContainsKey(chain[0]))
                        dict[chain[0]].Add(chain[1]);
                    else
                        dict.Add(chain[0],new List<string> {chain[1]});
                }
            return dict;
        }
    }
}
