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
        public static bool createUser(MySqlConnection dbcon, string username, string password, string email, string app, string dob, string gender)
        {
            try
            {
                dbcon.Open();
                //(ID, name, password, app, badgeonoff, badges, consolemission, birth, credits, curbadge, email, favorites, figure, film, friendlist, hand, hcdays, hcupdate, inquiries, lastonline, mission, num, periods, rank, roomlist, rooms, sex, showexpire, tickets)
                string query = "INSERT INTO `users` VALUES (0, '" + username + "', '" + password + "', '" + app + "', '0', '0', 'New User', '" + dob + "', '5000', '0', '" + email + "', '0', '0', '0', '0', '0', '3', '0', '0', '0', 'I am a new user', '0', '0', 'habbo', '0', '0', '" + gender + "', '0', '2')";
                Console.WriteLine(query);
                MySqlCommand command = new MySqlCommand(query, dbcon);
                command.ExecuteNonQuery();
                dbcon.Close();
                return true;
            }
            catch (Exception ex)
            {
                ConsoleLog.LogErrorLine(ex.Message);
                dbcon.Close();
                return false;
            }
        }
    }
}
