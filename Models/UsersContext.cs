using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace OzonTask.Models
{
    public class UsersContext
    {
        string connectionString = null;
        public UsersContext(string connectionString_)
        {
            connectionString = connectionString_;
        }


        public int GetUser(string emailAddress)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                 return connection.ExecuteScalar<int>("SELECT COUNT(\"Id\") FROM public.\"Users\" WHERE \"EmailAddress\"=\'" + emailAddress+"\'");
            }
        }

        public void AddUser(User u)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                int id = connection.ExecuteScalar<int>("SELECT COUNT(\"Id\") FROM public.\"Users\"") + 1;

                var sqlQuery = $"INSERT INTO public.\"Users\" VALUES({id}, \'{u.EmailAddress}\', \'{u.Pass}\')";
                connection.Execute(sqlQuery);
            }
        }
    }
}
