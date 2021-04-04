using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace OzonTask.Models
{
    public class EmailsContext
    {
        string connectionString = null;
        public User user { get; }

        public EmailsContext(string connString, User u)
        {
            connectionString = connString;
            user = u;
        }

        // Если нет получателей копии писма
        public void AddEmail(Email mail, string login, bool result)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                int id = connection.ExecuteScalar<int>("SELECT COUNT(\"Id\") FROM public.\"Emails\"") + 1;

                var sqlQuery = $"INSERT INTO public.\"Emails\" VALUES({id}, \'{login}\', \'{mail.recipient}\', \'{mail.subject}\', \'{mail.text}\'," +
                    $" \'{{NULL}}\' ,\'{result}\')";
                connection.Execute(sqlQuery);
            }
        }

        // Если есть
        public void AddEmail(Email mail, string currentCopyRecipient, string login, bool result)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                int id = connection.ExecuteScalar<int>("SELECT COUNT(\"Id\") FROM public.\"Emails\"") + 1;

                var sqlQuery = $"INSERT INTO public.\"Emails\" VALUES({id}, \'{login}\', \'{currentCopyRecipient}\', \'{mail.subject}\', \'{mail.text}\'," +
                    $" \'{{{String.Join(',', mail.carbon_copy_recipients)}}}\' ,\'{result}\')";
                connection.Execute(sqlQuery);
            }
        }

        public List<EmailResponse> GetEmails()
        {
            List<EmailResponse> resList = new List<EmailResponse>();
            EmailResponse resultResponse;
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string sql = "SELECT * FROM public.\"Emails\"";
            using(var cmd  = new NpgsqlCommand(sql, connection))
            {
                using(NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        resultResponse = new EmailResponse();
                        resultResponse.id = rdr.GetInt32(0);
                        resultResponse.sender = rdr.GetString(1);
                        resultResponse.recipient = rdr.GetString(2);
                        resultResponse.subject = rdr.GetString(3);
                        resultResponse.text = rdr.GetString(4);
                        resultResponse.carbon_copy_recipients = (string [])rdr.GetValue(5);
                        resultResponse.result = bool.Parse( rdr.GetValue(6).ToString() );
                        resList.Add(resultResponse);
                    }
                }
            }
            return resList;

        } 
    }
}
;