using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly string connectionString;

        public MatchRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Models.Match> GetMatchesByUserId(int userId)
        {
            var matches = new List<Match>();

            const string getMatchesByUserIdQuery = "SELECT id, userID, companyName, jobRole, matchDate FROM MATCHES WHERE userID = @UserId ORDER BY matchDate DESC";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(getMatchesByUserIdQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    matches.Add(new Match
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        UserId = Convert.ToInt32(reader["userID"]),
                        CompanyName = reader["companyName"].ToString(),
                        JobRole = reader["jobRole"].ToString(),
                        MatchDate = Convert.ToDateTime(reader["matchDate"]),
                    });
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error retrieving matches for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred retrieving matches for user {userId}: {ex.Message}");
            }

            return matches;
        }
    }
}
