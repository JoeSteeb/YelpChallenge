using System;
using Npgsql;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryHandler_
{
    public class QueryHandler
    {
        private string connection_str = "Host = localhost; Username = postgres; Database = yelpdb; password=NissanRogue#7";
        private string query;
        private string state;
        private string city;
        private string zip;

        public QueryHandler() 
        {
            query = string.Empty;
            state = string.Empty;
            city = string.Empty;
            zip = string.Empty;
        }

        public void ExecuteQuery(Action<NpgsqlDataReader> sql_reader)
        {
            using (var connection = new NpgsqlConnection(connection_str))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = query;
                    try
                    {
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                            sql_reader(reader);
                    }
                    catch (NpgsqlException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                        MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void SetQuery(string query)
        {
            this.query = query;
        }
    }
}
