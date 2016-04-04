using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.String;

namespace ConsoleTest
{
    public class DofusRessource
    {
        private static string TableName = "ressource";
        public int Level { get; set; }

        public string Nom { get; set; }

        public string Categorie { get; set; }

        public string Description { get; set; }

        public string ToSQL()
        {
            if (IsNullOrWhiteSpace(Nom) || IsNullOrWhiteSpace(Categorie) || IsNullOrWhiteSpace(Description))
                return "";
            return "INSERT OR REPLACE INTO " + TableName + " VALUES('" + Nom + "'," + Level + ",'" + Categorie +
                       "','" + Description + "')";
        }
    }

}
