using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib.Explore
{
    public class ExplorationFilter
    {
        public List<Uri> ValidRelativeUriList = new List<Uri>();
        
        public List<string> ValidGetList = new List<string>();

        public bool exploreDiesePage = false;

        public bool Validate(Uri uriToValidate)
        {
            if (!uriToValidate.IsAbsoluteUri)
                return true;
            string href = uriToValidate.AbsoluteUri;
            var getParam = uriToValidate.ParseQueryString();
            foreach (var uri in ValidRelativeUriList)
            {
                if (uri.IsBaseOf(uriToValidate))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
