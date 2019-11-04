using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _07._Print_All_Minion_Names
{
    public class StartUp
    {
        private static string connectionString = @"Server=.;" +
                                            "Database=MinionsDB;" +
                                            "Integrated Security = true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            connection.Open();

            using (connection)
            {
                string queryText = @"SELECT [Name]
	                                    FROM Minions";
                SqlCommand command = new SqlCommand(queryText, connection);
                //Open reader
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    //Insert minions name in List for manipulating more easy
                    List<string> minions = new List<string>();
                    while (reader.Read())
                    {
                        minions.Add((string)reader["Name"]);
                    }
                    //Printing
                    //Devide by to lengh because we print by couple
                    for (int index = 0; index < minions.Count / 2; index++)
                    {
                        Console.WriteLine($"{minions[index]}");
                        Console.WriteLine($"{minions[minions.Count - 1 - index]}");
                    }
                }
            }
        }
    }
}
