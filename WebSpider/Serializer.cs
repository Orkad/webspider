using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebSpiderLib
{
    public static class Serializer
    {
        /// <summary>
        ///     Fonction de sauvegarde d'objet dans un fichier (!!!) L'objet passé en paramètre doit avoir l'attribut [Serializable]
        /// </summary>
        /// <param name="path">chemin complet vers le fichier</param>
        /// <param name="objToSave">objet à sauvegarder (!!!) doit être [Serializable]</param>
        public static void Save(string path, object objToSave)
        {
            //Ouverture du stream & Serialisation de l'objet au format binaire
            using (Stream stream = File.Create(path))
                new BinaryFormatter().Serialize(stream, objToSave);
        }

        public static void SaveXml(string path, object objToSave)
        {
            //Ouverture du stream & Serialisation de l'objet au format binaire
            using (Stream stream = File.Create(path))
                new XmlSerializer(objToSave.GetType()).Serialize(stream, objToSave);
        }

        /// <summary>
        ///     Fonction de chargement d'objet depuis un fichier (!!!) Cast necessaire après cette fonction
        /// </summary>
        /// <param name="path">chemin complet vers le fichier</param>
        /// <returns></returns>
        public static object Load(string path)
        {
            //Overture du stream & Deserialisation du fichier au format binaire
            using (Stream stream = File.Open(path, FileMode.Open))
                return new BinaryFormatter().Deserialize(stream);
        }
    }
}
