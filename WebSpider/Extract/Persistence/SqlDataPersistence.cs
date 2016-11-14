using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace WebSpiderLib.Extract.Persistence
{
    public class SqlDataPersistence : IDataPersistence
    {
        private readonly MySqlConnection _connection;
        private readonly DataDefinition _definition;

        public SqlDataPersistence(string connexionString, DataDefinition definition)
        {
            _connection = new MySqlConnection(connexionString);
            _definition = definition;
            _connection.Open();
        }

        public void Persist(Data data)
        {
            try
            {
                string table = _definition.Name;
                string fields = string.Join(",", _definition.FieldDefinition.Select(definition => definition.Name));
                string values = string.Join(",", data.Fields.Select(field => "\"" + MySqlHelper.EscapeString(field.Value) + "\""));
                string query = "INSERT INTO " + table + "(" + fields + ") VALUES(" + values + ");";
                MySqlCommand insertDataCmd =  new MySqlCommand(query, _connection);
                insertDataCmd.Prepare();
                insertDataCmd.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}