using System;
using System.Data.SqlClient;
using System.Linq;

namespace _04._Add_Minion
{   
    public class StartUp
    {
        private static string connectionString = @"Server=.;" +
                                            "Database=MinionsDB;" +
                                            "Integrated Security = true;";
        private static SqlConnection connection = new SqlConnection(connectionString);
        static void Main(string[] args)
        { 
            string[] input = Console.ReadLine().Split(": ").ToArray();
            string[] minionInput = input[1].Split(' ').ToArray();

            string minionName = minionInput[0];
            int minionAge = int.Parse(minionInput[1]);
            string minionTown = string.Empty;
            if (minionInput.Length>3)
            {
                minionTown = minionInput[2];
                for (int index = 2; index < minionInput.Length-1; index++)
                {
                    minionTown += " " + minionInput[index+1];
                }
                minionTown.TrimEnd();
            }
            else
            {
                minionTown = minionInput[2];
            }
            

            input = Console.ReadLine().Split(": ").ToArray();
            string villainName = input[1];

            connection.Open();

            using (connection)
            {
                //Town check
                string queryString = @"SELECT [Name] 
	                                    FROM Towns
	                                    WHERE [Name] = @TownName";

                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@TownName", minionTown);

                object town = command.ExecuteScalar();

                //If Town is not in database
                if (town == null)
                {
                    string queryInsert = @"INSERT INTO Towns([Name], CountryCode) 
                                            VALUES (@TownName, 5)";
                    SqlCommand insertCommand = new SqlCommand(queryInsert, connection);
                    insertCommand.Parameters.AddWithValue("@TownName", minionTown);

                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"Town {minionTown} was added to the database.");
                }

                //Villain check
                string queryVillians = @"SELECT [Name]
	                                        FROM Villains
	                                        WHERE [Name] = @VillainName";
                SqlCommand villainCheck = new SqlCommand(queryVillians, connection);
                villainCheck.Parameters.AddWithValue("@VillainName", villainName);

                object villain = (string)villainCheck.ExecuteScalar();

                //If Villain dont exist in the database
                if (villain==null)
                {
                    string villainInsert = @"INSERT INTO Villains ([Name], EvilnessFactorId) VALUES
                                                (@VillainName, 4)";

                    SqlCommand insertVillain = new SqlCommand(villainInsert, connection);
                    insertVillain.Parameters.AddWithValue("@VillainName", villainName);

                    insertVillain.ExecuteNonQuery();
                    Console.WriteLine($"Villain {villainName} was added to the database.");

                }
                //After we have villian and Town
                //Take Town Id
                string queryTownId = @"SELECT Id
	                                        FROM Towns
	                                        WHERE [Name] = @TownName";

                SqlCommand takeTownId = new SqlCommand(queryTownId, connection);
                takeTownId.Parameters.AddWithValue("@TownName", minionTown);

                int townId = (int)takeTownId.ExecuteScalar();

                //Insert into minions the new entry minion
                string insertNewMinionQuery = @"INSERT INTO Minions ([Name], [Age],[TownId]) VALUES
                                                (@MinnionName, @MinnionAge, @TownId)";
                //Command for inserting minion
                SqlCommand insertNewMinnion = new SqlCommand(insertNewMinionQuery, connection);
                insertNewMinnion.Parameters.AddWithValue("@MinnionName", minionName);
                insertNewMinnion.Parameters.AddWithValue("@MinnionAge", minionAge);
                insertNewMinnion.Parameters.AddWithValue("@TownId", townId);

                //Take Minion Id
                string queryMinionId = @"SELECT Id
	                                        FROM Minions
	                                        WHERE [Name] = @MinnionName";

                SqlCommand takeMinionId = new SqlCommand(queryMinionId, connection);
                takeMinionId.Parameters.AddWithValue("@MinnionName", minionName);

                int MinionId = (int)takeMinionId.ExecuteScalar();

                //Take Villain Id
                string queryVillainId = @"SELECT Id
	                                        FROM Villains
	                                        WHERE [Name] = @Villain";

                SqlCommand takeVillainId = new SqlCommand(queryVillainId, connection);
                takeVillainId.Parameters.AddWithValue("@Villain", villainName);

                int VillainId = (int)takeVillainId.ExecuteScalar();

                //Assaign minion to villain

                string assaighQuery = @"INSERT INTO MinionsVillains ([MinionId],[VillainId]) VALUES
                                            (@MinnionId, @VillainId)";
                SqlCommand assaignCommand = new SqlCommand(assaighQuery, connection);
                assaignCommand.Parameters.AddWithValue("@MinnionId", MinionId);
                assaignCommand.Parameters.AddWithValue("@VillainId", VillainId);

                assaignCommand.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}
