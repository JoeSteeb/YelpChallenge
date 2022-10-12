using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace QueryHandler_
{
    public class PreparedStatement
    {
        NpgsqlCommand command;

        public PreparedStatement(NpgsqlConnection connection, string sql, Dictionary<string, NpgsqlDbType> parameters)
        {
            command = new NpgsqlCommand(sql, connection);
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter.Key, parameter.Value);
            }
            command.Prepare();
        }
        public PreparedStatement(NpgsqlConnection connection, string sql) : this(connection, sql, new Dictionary<string, NpgsqlDbType>()) { }


        public void SetParameters(Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters[parameter.Key].Value = parameter.Value;
            }
        }

        public IEnumerable<T> Query<T>(Func<NpgsqlDataReader, T> getData)
        {
            return Query(getData, new Dictionary<string, object>());
        }
        public IEnumerable<T> Query<T>(Func<NpgsqlDataReader, T> getData, Dictionary<string, object> parameters)
        {
            SetParameters(parameters);
            return Process(command.ExecuteReader(), getData);
        }

        public int Run()
        {
            return Run(new Dictionary<string, object>());
        }
        public int Run(Dictionary<string, object> parameters)
        {
            SetParameters(parameters);
            return command.ExecuteNonQuery();
        }
        public IEnumerable<T> Process<T>(NpgsqlDataReader reader, Func<NpgsqlDataReader, T> getData)
        {
            try
            {
                while (reader.Read())
                {
                    T data = getData(reader);
                    yield return data;
                }
            }
            finally
            {
                reader.Close();
            }
        }

    }
}
