using System;
using System.Data.SqlClient;

namespace _09._Increase_Age_Stored_Procedure
{
    public class StartUp
    {
        private static string connectionString = @"Server=.;" +
                                            "Database=MinionsDB;" +
                                            "Integrated Security = true;";

        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            int minionId = int.Parse(Console.ReadLine());
            connection.Open();

            using (connection)
            {
                //Use Procedure usp_GetOlder
                SqlCommand command = new SqlCommand("usp_GetOlder", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                //Give it value
                command.Parameters.AddWithValue("@MinionId", minionId);

                command.ExecuteNonQuery();

                //Find the update minion
                string queryPrint = @"SELECT [Name], [Age]
	                                    FROM Minions
	                                    WHERE Id = @MinionId";
                SqlCommand readerCmd = new SqlCommand(queryPrint, connection);
                readerCmd.Parameters.AddWithValue("@MinionId", minionId);

                //Open Reader
                SqlDataReader reader = readerCmd.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        //Print the minion
                        Console.WriteLine($"{reader["Name"]} - {reader["Age"]} years old");
                    }
                }
            }
        }
    }
}
