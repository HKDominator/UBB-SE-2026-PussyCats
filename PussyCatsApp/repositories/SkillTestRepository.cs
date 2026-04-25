using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class SkillTestRepository : ISkillTestRepository
    {
        private readonly string connectionString;
        public SkillTestRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SkillTest Load(int skillId)
        {
            const string selectSkillByIdQuery = "SELECT * FROM SKILLS WHERE skillID = @id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(selectSkillByIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", skillId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int skillIdToLoad = (int)reader["skillID"];
                                int userId = (int)reader["userID"];
                                string name = reader["name"].ToString();
                                int score = (int)reader["score"];

                                DateOnly dateResult;
                                if (reader["achievedDate"] != DBNull.Value)
                                {
                                    DateTime dbDate = (DateTime)reader["achievedDate"];
                                    dateResult = DateOnly.FromDateTime(dbDate);
                                }
                                else
                                {
                                    dateResult = default;
                                }

                                return new SkillTest(skillIdToLoad, userId, name, score, dateResult);
                            }
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine("Database error loading skill " + skillId + ": " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("An error occurred loading skill " + skillId + ": " + exception.Message);
            }

            throw new Exception("SkillTest with ID " + skillId + " not found.");
        }

        public void Save(int skillId, SkillTest data)
        {
            const string updateSkillByIdQuery = "UPDATE SKILLS SET score = @score, achievedDate = @date WHERE skillID = @id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(updateSkillByIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@score", data.Score);
                        command.Parameters.AddWithValue("@date", data.AchievedDate);
                        command.Parameters.AddWithValue("@id", skillId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine("Database error saving skill " + skillId + ": " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("An error occurred saving skill " + skillId + ": " + exception.Message);
            }
        }

        public List<SkillTest> GetSkillTestsByUserId(int userId)
        {
            List<SkillTest> skillTests = new List<SkillTest>();
            const string selectAllSkillsByUserIdQuery = "SELECT * FROM SKILLS WHERE userID = @userId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(selectAllSkillsByUserIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int skillId = (int)reader["skillID"];
                                int userIdToGet = (int)reader["userID"];
                                string name = reader["name"].ToString();
                                int score = (int)reader["score"];

                                DateOnly dateResult;
                                if (reader["achievedDate"] != DBNull.Value)
                                {
                                    DateTime dbDate = (DateTime)reader["achievedDate"];
                                    dateResult = DateOnly.FromDateTime(dbDate);
                                }
                                else
                                {
                                    dateResult = default;
                                }

                                SkillTest test = new SkillTest(skillId, userIdToGet, name, score, dateResult);
                                skillTests.Add(test);
                            }
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine("Database error retrieving skills for user " + userId + ": " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("An error occurred retrieving skills for user " + userId + ": " + exception.Message);
            }

            return skillTests;
        }

        public void UpdateSkillTestScore(int skillId, int score)
        {
            const string updateSkillScoreByIdQuery = "UPDATE SKILLS SET score = @score WHERE skillID = @id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(updateSkillScoreByIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@score", score);
                        command.Parameters.AddWithValue("@id", skillId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine("Database error updating score for skill " + skillId + ": " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("An error occurred updating score for skill " + skillId + ": " + exception.Message);
            }
        }

        public void UpdateAchievedDate(int skillId, DateOnly date)
        {
            const string updateSkillAchivedDateByIdQuery = "UPDATE SKILLS SET achievedDate = @date WHERE skillID = @id";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(updateSkillAchivedDateByIdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@date", date);
                        command.Parameters.AddWithValue("@id", skillId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine("Database error updating achieved date for skill " + skillId + ": " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("An error occurred updating achieved date for skill " + skillId + ": " + exception.Message);
            }
        }
    }
}