using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly string connectionString;

        public UserProfileRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private record ParsedCVData(
            List<WorkExperience> workExperiences,
            List<Project> projects,
            List<ExtraCurricularActivity> extraCurricularActivities);

        private record FormDataSnapshot(
            string firstName,
            string lastName,
            int age,
            string gender,
            string email,
            string phoneNumber,
            string gitHub,
            string linkedIn,
            string country,
            string city,
            string university,
            string degree,
            int universityStartYear,
            int expectedGraduationYear,
            string address,
            string motivation,
            bool hasDisabilities,
            List<string> skills,
            List<WorkExperience> workExperiences,
            List<Project> projects,
            List<ExtraCurricularActivity> extraCurricularActivities);

        public UserProfile GetProfileById(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                Debug.WriteLine("Database connection opened successfully.");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to connect to database.{e.Message}");
                return null;
            }

            try
            {
                UserProfile profile = LoadUserRow(connection, userId);
                Debug.WriteLine($"Loaded user row for userId={userId}: {(profile == null ? "NOT FOUND" : "FOUND")}");
                if (profile == null)
                {
                    return null;
                }

                profile.RelevantCertificates = LoadCertificates(connection, userId);
                LoadPreferences(connection, userId, profile);
                LoadFormData(connection, userId, profile);

                return profile;
            }
            catch (SqlException e)
            {
                Debug.WriteLine($"SQL Exception: {e.Message}");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public UserProfile Load(int id)
        {
            return null;
        }

        public void Save(int id, UserProfile profileData)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            try
            {
                using var dbTransaction = connection.BeginTransaction();
                UpsertUserRow(connection, dbTransaction, id, profileData);
                dbTransaction.Commit();
            }
            catch (SqlException e)
            {
                Debug.WriteLine($"SQL Exception: {e.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void UpdateAccountStatus(int userId, string status)
        {
            const string query = "UPDATE Users SET activeAccount = @status WHERE userID = @userId";
            bool isAccountActive = false;
            if (status == "ACTIVE")
            {
                isAccountActive = true;
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", isAccountActive);
                command.Parameters.AddWithValue("@userId", userId);

                int rowsAffectedCount = command.ExecuteNonQuery();
                if (rowsAffectedCount == 0)
                {
                    Console.WriteLine($"No user found with ID {userId} to update account status");
                }
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error updating account status for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred updating account status for user {userId}: {ex.Message}");
            }
        }

        public void UpdateProfileLastModified(int userId, DateTime newTimestamp)
        {
            const string query = "UPDATE Users SET LastUpdated = @time WHERE userID = @userId";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@time", newTimestamp);
                cmd.Parameters.AddWithValue("@userId", userId);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Database error updating LastModified for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred updating LastModified for user {userId}: {ex.Message}");
            }
        }

        public void UpdateProfilePicture(int userId, string profilePicturePath)
        {
            const string query = "UPDATE Users SET profilePicture = @path WHERE userID = @userId";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(query, connection);
                object pathValue = DBNull.Value;
                if (profilePicturePath != null)
                {
                    pathValue = profilePicturePath;
                }
                command.Parameters.AddWithValue("@path", pathValue);
                command.Parameters.AddWithValue("@userId", userId);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.Error.WriteLine($"Database error updating profile picture for user {userId}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred updating profile picture for user {userId}: {ex.Message}");
            }
        }

        private static UserProfile LoadUserRow(SqlConnection connection, int userId)
        {
            using var sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = @"
                SELECT userID, firstName, lastName, gender, age,
                       email, phone, github, linkedin, universityStartYear,
                       graduationYear, country, city, address, motivation,
                       disabilities,
                       personalityTestResult, activeAccount,
                       profilePicture, university, degree, LastUpdated, parsedCV,
                       formDataJson
                FROM Users
                WHERE userID = @id";
            sqlCommand.Parameters.AddWithValue("@id", userId);

            using var dataReader = sqlCommand.ExecuteReader();
            if (dataReader.Read() == false)
            {
                return null;
            }

            string rawGenderValue = GetString(dataReader, "gender").Trim();
            string genderToDisplay;

            switch (rawGenderValue)
            {
                case "M":
                    genderToDisplay = "Male";
                    break;
                case "F":
                    genderToDisplay = "Female";
                    break;
                default:
                    genderToDisplay = rawGenderValue;
                    break;
            }

            UserProfile profile = new UserProfile();
            profile.UserId = dataReader.GetInt32(dataReader.GetOrdinal("userID"));
            profile.FirstName = GetString(dataReader, "firstName");
            profile.LastName = GetString(dataReader, "lastName");
            profile.Gender = genderToDisplay;
            profile.Age = GetInt(dataReader, "age");
            profile.Email = GetString(dataReader, "email");
            profile.PhoneNumber = GetString(dataReader, "phone");
            profile.GitHub = GetString(dataReader, "github");
            profile.LinkedIn = GetString(dataReader, "linkedin");
            profile.UniversityStartYear = GetInt(dataReader, "universityStartYear");
            profile.ExpectedGraduationYear = GetInt(dataReader, "graduationYear");
            profile.Country = GetString(dataReader, "country");
            profile.City = GetString(dataReader, "city");
            profile.Address = GetString(dataReader, "address");
            profile.Motivation = GetString(dataReader, "motivation");

            bool disabilitiesFlag = false;
            int disabilitiesOrdinal = dataReader.GetOrdinal("disabilities");
            if (dataReader.IsDBNull(disabilitiesOrdinal) == false)
            {
                disabilitiesFlag = dataReader.GetBoolean(disabilitiesOrdinal);
            }
            profile.HasDisabilities = disabilitiesFlag;

            profile.University = GetString(dataReader, "university");
            profile.Degree = GetString(dataReader, "degree");
            profile.PersonalityTestResult = GetString(dataReader, "personalityTestResult");
            profile.ActiveAccount = dataReader.GetBoolean(dataReader.GetOrdinal("activeAccount"));

            DateTime lastUpdatedTimestamp;
            int lastUpdatedOrdinal = dataReader.GetOrdinal("LastUpdated");
            if (dataReader.IsDBNull(lastUpdatedOrdinal))
            {
                lastUpdatedTimestamp = DateTime.Now;
            }
            else
            {
                lastUpdatedTimestamp = dataReader.GetDateTime(lastUpdatedOrdinal);
            }
            profile.LastUpdated = lastUpdatedTimestamp;

            profile.ProfilePicture = GetString(dataReader, "profilePicture");
            profile.FormDataJson = GetString(dataReader, "formDataJson");

            return profile;
        }

        private static void LoadFormData(SqlConnection connection, int userId, UserProfile profile)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT formDataJson FROM Users WHERE userID = @id";
            command.Parameters.AddWithValue("@id", userId);

            var raw = command.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(raw))
                {
                    return;
                }

            try
            {
                FormDataSnapshot formData = JsonSerializer.Deserialize<FormDataSnapshot>(raw, JsonOptions);
                if (formData == null)
                {
                    return;
                }

                if (formData.skills != null)
                {
                    profile.Skills = formData.skills;
                }
                else
                {
                    profile.Skills = new List<string>();
                }

                if (formData.workExperiences != null)
                {
                    profile.WorkExperiences = formData.workExperiences;
                }
                else
                {
                    profile.WorkExperiences = new List<WorkExperience>();
                }

                if (formData.projects != null)
                {
                    profile.Projects = formData.projects;
                }
                else
                {
                    profile.Projects = new List<Project>();
                }

                if (formData.extraCurricularActivities != null)
                {
                    profile.ExtraCurricularActivities = formData.extraCurricularActivities;
                }
                else
                {
                    profile.ExtraCurricularActivities = new List<ExtraCurricularActivity>();
                }
            }
            catch (JsonException)
            {
                profile.Skills = new List<string>();
                profile.WorkExperiences = new List<WorkExperience>();
                profile.Projects = new List<Project>();
                profile.ExtraCurricularActivities = new List<ExtraCurricularActivity>();
            }
        }

        private static List<string> LoadCertificates(SqlConnection connection, int userId)
        {
            var list = new List<string>();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT nameDocument
                FROM Documents
                WHERE userID = @id
                ORDER BY dID";
            command.Parameters.AddWithValue("@id", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = GetString(reader, "nameDocument");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    list.Add(name);
                }
            }
            return list;
        }

        private static void LoadPreferences(SqlConnection connection, int userId, UserProfile profile)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT preferanceType, value
                FROM Preferences
                WHERE userID = @id";
            cmd.Parameters.AddWithValue("@id", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var type = GetString(reader, "preferanceType");
                var value = GetString(reader, "value");

                switch (type)
                {
                    case "JobRole":
                        profile.PreferredJobRoles.Add(value);
                        break;
                    case "WorkMode":
                        profile.WorkModePreference = value;
                        break;
                    case "Location":
                        profile.LocationPreference = value;
                        break;
                }
            }
        }

        private static void UpsertUserRow(SqlConnection connection, SqlTransaction transaction, int userId, UserProfile profile)
        {
            List<string> skillList = profile.Skills;
            if (skillList == null)
            {
                skillList = new List<string>();
            }

            List<WorkExperience> workList = profile.WorkExperiences;
            if (workList == null)
            {
                workList = new List<WorkExperience>();
            }

            List<Project> projectList = profile.Projects;
            if (projectList == null)
            {
                projectList = new List<Project>();
            }

            List<ExtraCurricularActivity> activityList = profile.ExtraCurricularActivities;
            if (activityList == null)
            {
                activityList = new List<ExtraCurricularActivity>();
            }

            FormDataSnapshot snapshot = new FormDataSnapshot(
                profile.FirstName, profile.LastName, profile.Age, profile.Gender,
                profile.Email, profile.PhoneNumber, profile.GitHub, profile.LinkedIn,
                profile.Country, profile.City, profile.University, profile.Degree,
                profile.UniversityStartYear, profile.ExpectedGraduationYear,
                profile.Address, profile.Motivation, profile.HasDisabilities,
                skillList, workList, projectList, activityList);

            string formDataJsonString = JsonSerializer.Serialize(snapshot, JsonOptions);

            using var sqlCommand = connection.CreateCommand();
            sqlCommand.Transaction = transaction;
            sqlCommand.CommandText = @"
                IF EXISTS (SELECT 1 FROM Users WHERE userID = @id)
                    UPDATE Users SET
                        firstName             = @firstName,
                        lastName              = @lastName,
                        gender                = @gender,
                        age                   = @age,
                        email                 = @email,
                        phone                 = @phone,
                        github                = @github,
                        linkedin              = @linkedin,
                        universityStartYear   = @universityStartYear,
                        graduationYear        = @graduationYear,
                        country               = @country,
                        city                  = @city,
                        address               = @address,
                        motivation            = @motivation,
                        disabilities          = @disabilities,
                        university            = @university,
                        degree                = @degree,
                        personalityTestResult = @personalityTestResult,
                        activeAccount         = @activeAccount,
                        profilePicture        = @profilePicture,
                        parsedCV              = @parsedCV,
                        formDataJson          = @formDataJson
                    WHERE userID = @id
                ELSE
                    INSERT INTO Users (
                        firstName, lastName, gender, age, email, phone,
                        github, linkedin, universityStartYear, graduationYear, country, city, address,
                        motivation, disabilities,
                        university, degree, personalityTestResult, activeAccount,
                        profilePicture, parsedCV, formDataJson
                    ) VALUES (
                        @firstName, @lastName, @gender, @age, @email, @phone,
                        @github, @linkedin, @universityStartYear, @graduationYear, @country, @city, @address,
                        @motivation, @disabilities,
                        @university, @degree, @personalityTestResult, @activeAccount,
                        @profilePicture, @parsedCV, @formDataJson
                    )";

            string genderDbValue;
            switch (profile.Gender)
            {
                case "Male":
                    genderDbValue = "M";
                    break;
                case "Female":
                    genderDbValue = "F";
                    break;
                default:
                    genderDbValue = profile.Gender;
                    break;
            }

            sqlCommand.Parameters.AddWithValue("@id", userId);
            sqlCommand.Parameters.AddWithValue("@firstName", profile.FirstName);
            sqlCommand.Parameters.AddWithValue("@lastName", profile.LastName);

            object genderParam = DBNull.Value;
            if (genderDbValue != null)
            {
                genderParam = genderDbValue;
            }
            sqlCommand.Parameters.AddWithValue("@gender", genderParam);

            sqlCommand.Parameters.AddWithValue("@age", profile.Age);
            sqlCommand.Parameters.AddWithValue("@email", profile.Email);

            sqlCommand.Parameters.AddWithValue("@phone", (object)profile.PhoneNumber ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@github", (object)profile.GitHub ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@linkedin", (object)profile.LinkedIn ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@universityStartYear", profile.UniversityStartYear);
            sqlCommand.Parameters.AddWithValue("@graduationYear", profile.ExpectedGraduationYear);
            sqlCommand.Parameters.AddWithValue("@country", (object)profile.Country ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@city", (object)profile.City ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@address", (object)profile.Address ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@motivation", (object)profile.Motivation ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@disabilities", profile.HasDisabilities);
            sqlCommand.Parameters.AddWithValue("@university", (object)profile.University ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@degree", (object)profile.Degree ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@personalityTestResult", (object)profile.PersonalityTestResult ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@activeAccount", profile.ActiveAccount);
            sqlCommand.Parameters.AddWithValue("@profilePicture", (object)profile.ProfilePicture ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@parsedCV", (object)profile.ParsedCV ?? DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@formDataJson", formDataJsonString);

            sqlCommand.ExecuteNonQuery();
        }

        // private static void SaveSkills(SqlConnection connection, SqlTransaction transaction,
        //    int userId, List<string> skills)
        // {
        //    using (var del = connection.CreateCommand())
        //    {
        //        del.Transaction = transaction;
        //        del.CommandText = "DELETE FROM Skills WHERE userID = @id";
        //        del.Parameters.AddWithValue("@id", userId);
        //        del.ExecuteNonQuery();
        //    }

        // foreach (var skill in skills ?? new List<string>())
        //    {
        //        using var cmd = connection.CreateCommand();
        //        cmd.Transaction = transaction;
        //        cmd.CommandText = "INSERT INTO Skills (userID, name) VALUES (@uid, @name)";
        //        cmd.Parameters.AddWithValue("@uid", userId);
        //        cmd.Parameters.AddWithValue("@name", skill);
        //        cmd.ExecuteNonQuery();
        //    }
        // }

        // private static void SavePreferences(SqlConnection connection, SqlTransaction transaction,
        //    int userId, UserProfile profile)
        // {
        //    using (var del = connection.CreateCommand())
        //    {
        //        del.Transaction = transaction;
        //        del.CommandText = "DELETE FROM Preferences WHERE userID = @id";
        //        del.Parameters.AddWithValue("@id", userId);
        //        del.ExecuteNonQuery();
        //    }

        // void Insert(string type, string value)
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            return;
        //        }
        //        using var cmd = connection.CreateCommand();
        //        cmd.Transaction = transaction;
        //        cmd.CommandText = @"
        //            INSERT INTO Preferences (userID, preferanceType, value)
        //            VALUES (@uid, @type, @value)";
        //        cmd.Parameters.AddWithValue("@uid", userId);
        //        cmd.Parameters.AddWithValue("@type", type);
        //        cmd.Parameters.AddWithValue("@value", value);
        //        cmd.ExecuteNonQuery();
        //    }

        // foreach (var role in profile.PreferredJobRoles ?? new List<string>())
        //    {
        //        Insert("JobRole", role);
        //    }

        // Insert("WorkMode", profile.WorkModePreference);
        // Insert("Location", profile.LocationPreference);
        // }
        private static string GetString(SqlDataReader reader, string columnName)
        {
            int ordinalIndex = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinalIndex))
            {
                return string.Empty;
            }
            return reader.GetString(ordinalIndex);
        }
        private static int GetInt(SqlDataReader reader, string columnName)
        {
            int ordinalIndex = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinalIndex))
            {
                return 0;
            }
            object columnValue = reader.GetValue(ordinalIndex);
            return Convert.ToInt32(columnValue);
        }
    }
}