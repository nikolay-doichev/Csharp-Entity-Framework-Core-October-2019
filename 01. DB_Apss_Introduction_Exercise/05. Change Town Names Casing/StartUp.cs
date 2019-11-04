using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace _05._Change_Town_Names_Casing
{
    public class StartUp
    {
        private static string connectionString = @"Server=.;" +
                                            "Database=MinionsDB;" +
                                            "Integrated Security = true;" +
                                            "MultipleActiveResultSets=true;";
        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            connection.Open();

            using (connection)
            {
                //Search the country Query
                int rowsAffected = 0;
                string queryText = @"SELECT *
	                                    FROM Towns AS t
	                                    JOIN Countries AS c ON t.CountryCode = c.Id
	                                    WHERE c.[Name] = @Country";

                SqlCommand command = new SqlCommand(queryText, connection);
                command.Parameters.AddWithValue("@Country", countryName);

                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    //If there is such a country
                    if (reader.HasRows)
                    {
                        //Update towns
                        string queryChangeName = @"UPDATE Towns SET [Name] = UPPER([Name])
                                                    WHERE CountryCode = (SELECT Id
						                            FROM Countries
						                            WHERE [Name] = @Country)";

                        SqlCommand changeNameCmd = new SqlCommand(queryChangeName, connection);
                        changeNameCmd.Parameters.AddWithValue("@Country", countryName);
                        //Taking the number of affected rows and execute the query
                        rowsAffected = changeNameCmd.ExecuteNonQuery();

                        Console.WriteLine($"{rowsAffected} town names were affected.");
                    }
                    //If there isn`t such Country or don`t have towns in Database 
                    else if (rowsAffected == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                        return;
                    }
                }
                //Search for edit Towns
                string findEditedTownsQuery = @"SELECT t.[Name]
	                                                FROM Towns AS t
	                                                JOIN Countries AS c ON t.CountryCode = c.Id
	                                                WHERE c.[Name] = @Country";
                SqlCommand findTowns = new SqlCommand(findEditedTownsQuery, connection);
                findTowns.Parameters.AddWithValue("@Country", countryName);
                //Open reader
                SqlDataReader readerTowns = findTowns.ExecuteReader();

                using (readerTowns)
                {
                    //Insert towns in List for manipulating more easy
                    List<string> tows = new List<string>();
                    while (readerTowns.Read())
                    {
                        tows.Add((string)readerTowns["Name"]);
                    }
                    //Printing
                    Console.Write('[');
                    for (int index = 0; index < rowsAffected - 1; index++)
                    {
                        Console.Write($"{tows[index]}, ");
                    }
                    Console.Write($"{tows[tows.Count - 1]}]");
                }
            }
        }
    }
}