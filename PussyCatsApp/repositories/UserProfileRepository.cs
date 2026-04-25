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
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to connect to database.{exception.Message}");
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
            catch (SqlException exception)
            {
                Debug.WriteLine($"SQL Exception: {exception.Message}");
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
                using var databaseTransaction = connection.BeginTransaction();
                UpsertUserRow(connection, databaseTransaction, id, profileData);
                databaseTransaction.Commit();
            }
            catch (SqlException exception)
            {
                Debug.WriteLine($"SQL Exception: {exception.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void UpdateAccountStatus(int userId, string status)
        {
            const string updateAccountStatusByUserIdQuery = "UPDATE Users SET activeAccount = @status WHERE userID = @userId";
            bool isAccountActive = false;
            if (status == "ACTIVE")
            {
                isAccountActive = true;
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(updateAccountStatusByUserIdQuery, connection);
                command.Parameters.AddWithValue("@status", isAccountActive);
                command.Parameters.AddWithValue("@userId", userId);

                int rowsAffectedCount = command.ExecuteNonQuery();
                if (rowsAffectedCount == 0)
                {
                    Console.WriteLine($"No user found with ID {userId} to update account status");
                }
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine($"Database error updating account status for user {userId}: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"An error occurred updating account status for user {userId}: {exception.Message}");
            }
        }

        public void UpdateProfileLastModified(int userId, DateTime newTimestamp)
        {
            const string updateProfileLastModifiedDateByUserIdQuery = "UPDATE Users SET LastUpdated = @time WHERE userID = @userId";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(updateProfileLastModifiedDateByUserIdQuery, connection);
                command.Parameters.AddWithValue("@time", newTimestamp);
                command.Parameters.AddWithValue("@userId", userId);

                command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Debug.WriteLine($"Database error updating LastModified for user {userId}: {exception.Message}");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"An error occurred updating LastModified for user {userId}: {exception.Message}");
            }
        }

        public void UpdateProfilePicture(int userId, string profilePicturePath)
        {
            const string updateProfilePictureByUserIdQuery = "UPDATE Users SET profilePicture = @path WHERE userID = @userId";

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(updateProfilePictureByUserIdQuery, connection);
                object pathValue = DBNull.Value;
                if (profilePicturePath != null)
                {
                    pathValue = profilePicturePath;
                }
                command.Parameters.AddWithValue("@path", pathValue);
                command.Parameters.AddWithValue("@userId", userId);

                command.ExecuteNonQuery();
            }
            catch (SqlException exception)
            {
                Console.Error.WriteLine($"Database error updating profile picture for user {userId}: {exception.Message}");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"An error occurred updating profile picture for user {userId}: {exception.Message}");
            }
        }

        private static UserProfile LoadUserRow(SqlConnection connection, int userId)
        {
            using var loadUserByUserIdCommand = connection.CreateCommand();
            loadUserByUserIdCommand.CommandText = @"
                SELECT userID, firstName, lastName, gender, age,
                       email, phone, github, linkedin, universityStartYear,
                       graduationYear, country, city, address, motivation,
                       disabilities,
                       personalityTestResult, activeAccount,
                       profilePicture, university, degree, LastUpdated, parsedCV,
                       formDataJson
                FROM Users
                WHERE userID = @id";
            loadUserByUserIdCommand.Parameters.AddWithValue("@id", userId);

            using var dataReader = loadUserByUserIdCommand.ExecuteReader();
            if (dataReader.Read() == false)
            {
                return null;
            }

            string rawGenderValue = GetString(dataReader, "gender").Trim();
            string genderToDisplay;

            const string maleShortValue = "M";
            const string femaleShortValue = "F";
            switch (rawGenderValue)
            {
                case maleShortValue:
                    genderToDisplay = "Male";
                    break;
                case femaleShortValue:
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
            using var selectFormDataCommand = connection.CreateCommand();
            selectFormDataCommand.CommandText = "SELECT formDataJson FROM Users WHERE userID = @id";
            selectFormDataCommand.Parameters.AddWithValue("@id", userId);

            var rawFormData = selectFormDataCommand.ExecuteScalar() as string;
            if (string.IsNullOrWhiteSpace(rawFormData))
                {
                    return;
                }

            try
            {
                FormDataSnapshot formData = JsonSerializer.Deserialize<FormDataSnapshot>(rawFormData, JsonOptions);
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
            using var loadNameDocumentCommand = connection.CreateCommand();
            loadNameDocumentCommand.CommandText = @"
                SELECT nameDocument
                FROM Documents
                WHERE userID = @id
                ORDER BY dID";
            loadNameDocumentCommand.Parameters.AddWithValue("@id", userId);

            using var reader = loadNameDocumentCommand.ExecuteReader();
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
            using var selectPreferencesByUserIdCommand = connection.CreateCommand();
            selectPreferencesByUserIdCommand.CommandText = @"
                SELECT preferanceType, value
                FROM Preferences
                WHERE userID = @id";
            selectPreferencesByUserIdCommand.Parameters.AddWithValue("@id", userId);

            using var reader = selectPreferencesByUserIdCommand.ExecuteReader();
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

            using var updateOrInsertCommand = connection.CreateCommand();
            updateOrInsertCommand.Transaction = transaction;
            updateOrInsertCommand.CommandText = @"
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

            string genderDatabaseValue;
            switch (profile.Gender)
            {
                case "Male":
                    genderDatabaseValue = "M";
                    break;
                case "Female":
                    genderDatabaseValue = "F";
                    break;
                default:
                    genderDatabaseValue = profile.Gender;
                    break;
            }

            updateOrInsertCommand.Parameters.AddWithValue("@id", userId);
            updateOrInsertCommand.Parameters.AddWithValue("@firstName", profile.FirstName);
            updateOrInsertCommand.Parameters.AddWithValue("@lastName", profile.LastName);

            object genderParameter = DBNull.Value;
            if (genderDatabaseValue != null)
            {
                genderParameter = genderDatabaseValue;
            }
            updateOrInsertCommand.Parameters.AddWithValue("@gender", genderParameter);

            updateOrInsertCommand.Parameters.AddWithValue("@age", profile.Age);
            updateOrInsertCommand.Parameters.AddWithValue("@email", profile.Email);

            updateOrInsertCommand.Parameters.AddWithValue("@phone", (object)profile.PhoneNumber ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@github", (object)profile.GitHub ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@linkedin", (object)profile.LinkedIn ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@universityStartYear", profile.UniversityStartYear);
            updateOrInsertCommand.Parameters.AddWithValue("@graduationYear", profile.ExpectedGraduationYear);
            updateOrInsertCommand.Parameters.AddWithValue("@country", (object)profile.Country ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@city", (object)profile.City ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@address", (object)profile.Address ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@motivation", (object)profile.Motivation ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@disabilities", profile.HasDisabilities);
            updateOrInsertCommand.Parameters.AddWithValue("@university", (object)profile.University ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@degree", (object)profile.Degree ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@personalityTestResult", (object)profile.PersonalityTestResult ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@activeAccount", profile.ActiveAccount);
            updateOrInsertCommand.Parameters.AddWithValue("@profilePicture", (object)profile.ProfilePicture ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@parsedCV", (object)profile.ParsedCV ?? DBNull.Value);
            updateOrInsertCommand.Parameters.AddWithValue("@formDataJson", formDataJsonString);

            updateOrInsertCommand.ExecuteNonQuery();
        }

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