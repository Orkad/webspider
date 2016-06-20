using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib.Parsing
{
    [Serializable]
    public class Data
    {
        public List<Field> Fields = new List<Field>();

        internal Data() { }
    }
}
