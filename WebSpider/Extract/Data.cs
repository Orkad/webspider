using System;
using System.Collections.Generic;

namespace WebSpiderLib.Extract
{
    [Serializable]
    public class Data
    {
        public List<Field> Fields = new List<Field>();

        internal Data() { }
    }
}
