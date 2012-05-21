using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace JaguarProject.IO
{
    class DBConnection
    {
        public static string mysql_host;
        public static string mysql_db;
        public static string mysql_user;
        public static string mysql_user_pass;
        public MySqlConnection connection;

        public bool connect()
        {
            connection = new MySqlConnection("SERVER=" + mysql_host + ";DATABASE=" + mysql_db + ";UID=" + mysql_user + ";PASSWORD=" + mysql_user_pass + ";");
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }

            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        ConsoleLog.LogErrorLine("Unable to connect to MySQL server.");
                        break;
                    case 1045:
                        ConsoleLog.LogErrorLine("Invalid username/password combination for MySQL server.");
                        break;
                }
                connection.Close();
                return false;
            }
        }
        public void query(string dbQuery)
        {
            MySqlCommand command = new MySqlCommand(dbQuery, connection);
            
        }
    }
}
