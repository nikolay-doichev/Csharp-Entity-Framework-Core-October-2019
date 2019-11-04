using System;
using System.Data.SqlClient;
using System.Linq;

namespace _08._Increase_Minion_Age
{
    public class StartUp
    {
        private static string connectionString = @"Server=.;" +
                                            "Database=MinionsDB;" +
                                            "Integrated Security = true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            //Collect information
            string[] mionionsId = Console.ReadLine().Split(' ').ToArray();

            connection.Open();

            using (connection)
            {
                //Query for Update minions Age and Id
                string updateAgeAndNameQuery = @"UPDATE Minions 
                                         SET Age += 1, [Name] = CONCAT(UPPER(SUBSTRING([Name], 1,1))
                                         ,LOWER(SUBSTRING([Name], 2, LEN([Name]))))
	                                     WHERE Id IN (@Id)";

                SqlCommand command = new SqlCommand(updateAgeAndNameQuery, connection);
                //Foreche rotation for every parameter Id
                foreach (var Id in mionionsId)
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                //Open reader and show all minions in database
                string selectUpadtedMinions = @"SELECT [Name], [Age]
	                                                FROM Minions";
                SqlCommand printCmd = new SqlCommand(selectUpadtedMinions, connection);
                SqlDataReader reader = printCmd.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
                    }
                }
            }
        }
    }
}
