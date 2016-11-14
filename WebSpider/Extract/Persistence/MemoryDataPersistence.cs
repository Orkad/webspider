using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WebSpiderLib.Extract.Persistence
{
    public class MemoryDataPersistence : IDataPersistence
    {
        public List<Data> DataList { get; } = new List<Data>();

        public void Persist(Data data)
        {
            DataList.Add(data);
        }
    }
}