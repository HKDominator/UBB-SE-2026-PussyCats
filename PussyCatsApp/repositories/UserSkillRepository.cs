using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class UserSkillRepository : IUserSkillRepository
    {
        private readonly string connectionString;

        public UserSkillRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<UserSkill> GetVerifiedSkillsByUserId(int userId)
        {
            List<UserSkill> skills = new List<UserSkill>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectVerifiedSkillsByUserIdQuery = "SELECT name, score FROM SKILLS WHERE userID = @UserId";
                SqlCommand command = new SqlCommand(selectVerifiedSkillsByUserIdQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    UserSkill skill = new UserSkill
                    {
                        SkillName = reader["name"].ToString(),
                        IsVerified = true,
                        Score = (int)reader["score"]
                    };

                    skills.Add(skill);
                }
            }

            return skills;
        }

        public string GetParsedCvByUserId(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectParsedCvByUserIdQuery = "SELECT ParsedCV FROM Users WHERE userID = @UserId";
                SqlCommand command = new SqlCommand(selectParsedCvByUserIdQuery, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return null;
                }

                return result.ToString();
            }
        }
    }
}