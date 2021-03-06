﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib.Explore
{
    public class ExplorationFilter
    {
        public List<Uri> ValidBaseUriList = new List<Uri>();
        
        public List<string> ValidGetList = new List<string>();

        public bool exploreSharpPage = false;

        public bool Validate(Uri uriToValidate)
        {
            if (!uriToValidate.IsAbsoluteUri)
                return true;
            var getParam = uriToValidate.ParseQueryString();
            return ValidBaseUriList.Any(uri => uriToValidate.AbsoluteUri.Contains(uri.AbsoluteUri)) && getParam.All(get => ValidGetList.Contains(get.Key));
        }
    }
}
