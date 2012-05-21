using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JaguarProject.IO
{
    class DBFunctions
    {
        public static int userTaken(MySqlConnection dbcon, string userName)
        {
             dbcon.Open();
             string query = "SELECT * FROM `users` WHERE name = \"" + userName + "\"";
             MySqlCommand command = new MySqlCommand(query, dbcon);
             MySqlDataReader reader = command.ExecuteReader();
             if (reader.Read())
             {
                 dbcon.Close();
                 return 1;
             }
             else
             {
                 dbcon.Close();
                 return 0;
             }
        }
    }
}
