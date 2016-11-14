using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSpiderLib.Extract.Persistence
{
    public interface IDataPersistence
    {
        void Persist(Data data);
    }
}
