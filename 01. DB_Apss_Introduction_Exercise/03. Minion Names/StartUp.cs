using System;
using System.Data.SqlClient;

namespace _03._Minion_Names
{
    public class StartUp
    {
        private static string connectionString = 
            "Server=.;" + 
            "Database=MinionsDB;" +
            "Integrated Security = true;";
        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            connection.Open();

            using (connection)
            {
                string queryString = @"SELECT *	FROM Villains
	                                        WHERE Id = @Id";

                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Id", villainId);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Villain: {reader["Name"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                        return;
                    }
                   
                }

                queryString = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Id", villainId);

                reader = command.ExecuteReader();

                using (reader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["RowNum"]}. {reader["Name"]} {reader["Age"]}");
                        }
                    }

                    else
                    {
                        Console.WriteLine("(no minions)");
                    }
                }
            }
        }
    }
}
