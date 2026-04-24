using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Configuration;

namespace PussyCatsApp.Repositories.PersonalityTestRepo;

public class PersonalityTestRepository : IPersonalityTestRepository
{
    private readonly string connectionString;

    public PersonalityTestRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public string? Load(int id)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand selectPersonalityTestResultCommand = new SqlCommand("SELECT personalityTestResult FROM Users WHERE userID = @userID", connection))
                {
                    selectPersonalityTestResultCommand.Parameters.AddWithValue("@userID", id);
                    using (SqlDataReader reader = selectPersonalityTestResultCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["personalityTestResult"].ToString();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
            }
        }

        return null;
    }

    public void Save(int userId, string personalityTestResult)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                using (SqlCommand updatePersonalityTestResultByUserIdCommand = new SqlCommand("UPDATE Users SET personalityTestResult = @personalityTestResult WHERE userID = @userID", connection))
                {
                    updatePersonalityTestResultByUserIdCommand.Parameters.AddWithValue("@personalityTestResult", personalityTestResult);
                    updatePersonalityTestResultByUserIdCommand.Parameters.AddWithValue("@userID", userId);
                    updatePersonalityTestResultByUserIdCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
            }
        }
    }
}
