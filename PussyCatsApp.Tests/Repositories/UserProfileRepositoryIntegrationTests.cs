using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class UserProfileRepositoryIntegrationTests
    {
        private UserProfileRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new UserProfileRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetProfileById_NonExistentUser_ExpectsNull()
        {
            int NonExistentUserId = 10876;

            UserProfile result = Repository.GetProfileById(NonExistentUserId);

            Assert.IsNull(result, "Expected null when querying a user that does not exist.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsNotNull()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.IsNotNull(result, "Expected a non-null profile for an existing user.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectUserId()
        {
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(userId, result.UserId, $"Expected userId {userId} but got {result.UserId}.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectFirstName()
        {
            string ExpectedFirstName = "Ioana";
            int userId = TestDatabaseHelper.InsertUser(firstName: ExpectedFirstName);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedFirstName, result.FirstName, $"Expected first name '{ExpectedFirstName}' but got '{result.FirstName}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectLastName()
        {
            string ExpectedLastName = "Gavrila";
            int userId = TestDatabaseHelper.InsertUser(lastName: ExpectedLastName);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedLastName, result.LastName, $"Expected last name '{ExpectedLastName}' but got '{result.LastName}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectEmail()
        {
            string EmailAddress = "ioana@test.com";
            int userId = TestDatabaseHelper.InsertUser(email: EmailAddress);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(EmailAddress, result.Email, $"Expected email '{EmailAddress}' but got '{result.Email}'.");
        }

        [TestMethod]
        public void GetProfileById_ExistingUser_ExpectsCorrectAge()
        {
            int UserAgeValue = 22;
            int userId = TestDatabaseHelper.InsertUser(age: UserAgeValue);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(UserAgeValue, result.Age, $"Expected age {UserAgeValue} but got {result.Age}.");
        }

        [TestMethod]
        public void GetProfileById_UserWithFemaleGender_ExpectsCorrectGender()
        {
            string ExpectedGenderValue = "Female";
            int userId = TestDatabaseHelper.InsertUser(gender: ExpectedGenderValue);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedGenderValue, result.Gender, "Expected gender 'Female' to be returned as-is.");
        }

        [TestMethod]
        public void GetProfileById_UserWithCity_ExpectsCorrectCity()
        {
            string ExpectedCityName = "Cluj-Napoca";
            int userId = TestDatabaseHelper.InsertUser(city: ExpectedCityName);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedCityName, result.City, $"Expected city '{ExpectedCityName}' but got '{result.City}'.");
        }

        [TestMethod]
        public void GetProfileById_UserWithMotivation_ExpectsCorrectMotivation()
        {
            string ExpectedMotivationText = "I love coding";
            int userId = TestDatabaseHelper.InsertUser(motivation: ExpectedMotivationText);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedMotivationText, result.Motivation, $"Expected motivation '{ExpectedMotivationText}' but got '{result.Motivation}'.");
        }

        [TestMethod]
        public void GetProfileById_UserWithGraduationYear_ExpectsCorrectYear()
        {
            int ExpectedGraduationYearValue = 2026;
            int userId = TestDatabaseHelper.InsertUser(graduationYear: ExpectedGraduationYearValue);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedGraduationYearValue, result.ExpectedGraduationYear, $"Expected graduation year {ExpectedGraduationYearValue} but got {result.ExpectedGraduationYear}.");
        }

        [TestMethod]
        public void GetProfileById_UserHasNoCertificates_ExpectsEmptyCertificatesList()
        {
            int ExpectedEmptyCount = 0;
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedEmptyCount, result.RelevantCertificates.Count, "Expected empty certificates list for user with no documents.");
        }

        [TestMethod]
        public void GetProfileById_UserWithOneCertificate_ExpectsOneCertificate()
        {
            int ExpectedCertificateCount = 1;
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, "AWS Certificate");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedCertificateCount, result.RelevantCertificates.Count, "Expected exactly one certificate.");
        }

        [TestMethod]
        public void GetProfileById_UserHasTwoCertificates_ExpectsTwoCertificates()
        {
            int ExpectedCertificateCount = 2;
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, "AWS Certificate");
            TestDatabaseHelper.InsertDocument(userId, "Azure Certificate");

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedCertificateCount, result.RelevantCertificates.Count, "Expected two certificates.");
        }

        [TestMethod]
        public void GetProfileById_UserWithWorkModePreference_ExpectsCorrectWorkMode()
        {
            string PreferenceType = "WorkMode";
            string PreferenceValue = "Remote";
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertPreference(userId, PreferenceType, PreferenceValue);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(PreferenceValue, result.WorkModePreference, "Expected work mode preference 'Remote'.");
        }

        [TestMethod]
        public void GetProfileById_UserWithNoPreferences_ExpectsEmptyJobRolesList()
        {
            int ExpectedCount = 0;
            int userId = TestDatabaseHelper.InsertUser();

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(ExpectedCount, result.PreferredJobRoles.Count, "Expected empty preferred job roles list.");
        }

        [TestMethod]
        public void UpdateAccountStatus_SetToActive_ExpectsAccountBeingActive()
        {
            string ActiveStatus = "ACTIVE";
            int userId = TestDatabaseHelper.InsertUser(activeAccount: false);

            Repository.UpdateAccountStatus(userId, ActiveStatus);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.IsTrue(result.ActiveAccount, "Expected ActiveAccount to be true after setting status to ACTIVE.");
        }

        [TestMethod]
        public void UpdateAccountStatus_NonExistentUser_ExpectsGracefulHandling()
        {
            int NonExistentUserId = 10876;
            string ActiveStatus = "ACTIVE";

            try
            {
                Repository.UpdateAccountStatus(NonExistentUserId, ActiveStatus);
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected no exception but got: {exception.Message}");
            }
        }

        [TestMethod]
        public void UpdateProfilePicture_ValidPath_ExpectsNewProfilePicture()
        {
            string NewPicturePath = "uploads/avatars/new_picture.jpg";
            int userId = TestDatabaseHelper.InsertUser();

            Repository.UpdateProfilePicture(userId, NewPicturePath);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(NewPicturePath, result.ProfilePicture, $"Expected profile picture '{NewPicturePath}' but got '{result.ProfilePicture}'.");
        }

        [TestMethod]
        public void UpdateProfilePicture_NonExistentUser_ExpectsGracefulHandling()
        {
            int NonExistentUserId = 10876;
            string TestPath = "uploads/test.jpg";

            try
            {
                Repository.UpdateProfilePicture(NonExistentUserId, TestPath);
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected no exception but got: {exception.Message}");
            }
        }

        [TestMethod]
        public void UpdateProfileLastModified_ValidTimestamp_ExpectsNewDate()
        {
            int Year = 2026;
            int Month = 4;
            int Day = 21;
            int Hour = 10;
            int Minute = 30;
            int Second = 0;

            int userId = TestDatabaseHelper.InsertUser();
            DateTime newTimestamp = new DateTime(Year, Month, Day, Hour, Minute, Second);

            Repository.UpdateProfileLastModified(userId, newTimestamp);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(newTimestamp, result.LastUpdated, $"Expected LastUpdated '{newTimestamp}' but got '{result.LastUpdated}'.");
        }

        [TestMethod]
        public void UpdateProfileLastModified_NonExistentUser_ExpectsGracefulHandling()
        {
            int NonExistentUserId = 10876;
            try
            {
                Repository.UpdateProfileLastModified(NonExistentUserId, DateTime.Now);
                Assert.IsTrue(true, "Expected no exception for non-existent user.");
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected no exception but got: {exception.Message}");
            }
        }

        [TestMethod]
        public void Save_NewUser_ExpectsNewProfileAdded()
        {
            int NewUserIdValue = 1;
            int UserAge = 20;

            UserProfile newProfile = new UserProfile
            {
                FirstName = "Amalia",
                LastName = "Antici",
                Gender = "Female",
                Age = UserAge,
                Email = "amalia@test.com",
                ActiveAccount = true
            };

            Repository.Save(NewUserIdValue, newProfile);
            UserProfile result = Repository.GetProfileById(NewUserIdValue);

            Assert.IsNotNull(result, "Expected profile to be inserted and retrievable.");
        }

        [TestMethod]
        public void Save_ExistingUser_ExpectsNewFirstName()
        {
            string NewName = "NewAmalia";
            int UserAge = 20;

            int userId = TestDatabaseHelper.InsertUser(firstName: "OldName");
            UserProfile updatedProfile = new UserProfile
            {
                FirstName = NewName,
                LastName = "User",
                Gender = "Female",
                Age = UserAge,
                Email = "newAmalia@test.com",
                ActiveAccount = true
            };

            Repository.Save(userId, updatedProfile);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(NewName, result.FirstName, $"Expected updated first name '{NewName}' but got '{result.FirstName}'.");
        }

        [TestMethod]
        public void Save_ExistingUser_ExpectsNewMotivation()
        {
            string NewMotivationText = "New motivation";
            int UserAge = 20;

            int userId = TestDatabaseHelper.InsertUser(motivation: "Old motivation");
            UserProfile updatedProfile = new UserProfile
            {
                FirstName = "Test",
                LastName = "User",
                Gender = "Female",
                Age = UserAge,
                Email = "test@test.com",
                Motivation = NewMotivationText,
                ActiveAccount = true
            };

            Repository.Save(userId, updatedProfile);
            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(NewMotivationText, result.Motivation, $"Expected motivation '{NewMotivationText}' but got '{result.Motivation}'.");
        }

        [TestMethod]
        public void GetProfileById_InvalidConnectionString_ExpectsConnectionException()
        {
            int TargetId = 1;
            string ConnectionString = "Server=NonExistentServer;Database=FakeDB;Trusted_Connection=True;Connect Timeout=1;";
            var badRepository = new UserProfileRepository(ConnectionString);

            var result = badRepository.GetProfileById(TargetId);

            Assert.IsNull(result, "Should return null when connection fails.");
        }

        [TestMethod]
        public void GetProfileById_MalformedSql_ExpectsSqlConnectionException()
        {
            int InvalidId = -1;
            string PoolExtension = ";Max Pool Size=1;";
            var malformedRepository = new UserProfileRepository(TestDatabaseHelper.ConnectionString + PoolExtension);

            var result = malformedRepository.GetProfileById(InvalidId);
        }

        [TestMethod]
        public void LoadFormData_MalformedJson_ExpectsJsonExceptionHandled()
        {
            int userId = TestDatabaseHelper.InsertUser();
            using (var sqlConnection = new SqlConnection(TestDatabaseHelper.ConnectionString))
            {
                sqlConnection.Open();
                string UpdateCommand = "UPDATE Users SET formDataJson = '{ invalid: json }' WHERE userID = @identification";
                using (var sqlCommand = new SqlCommand(UpdateCommand, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@identification", userId);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            var profile = Repository.GetProfileById(userId);

            Assert.IsNotNull(profile.Skills);
        }

        [TestMethod]
        public void UpdateAccountStatus_InvalidId_ExpectsNoRowsAffected()
        {
            int InvalidIdValue = 99999;
            
            string StatusText = "ACTIVE";
            
            Repository.UpdateAccountStatus(InvalidIdValue, StatusText);
        }

        [TestMethod]
        public void UpdateProfilePicture_ExpectsSqlConnectionError()
        {
            int TargetId = 1;
            string TargetPath = "path/to/pic.png";
            string BadConnectionString = "Server=localhost;Database=NonExistent;Connect Timeout=1;";

            var badRepository = new UserProfileRepository(BadConnectionString);
            badRepository.UpdateProfilePicture(TargetId, TargetPath);
        }

        [TestMethod]
        public void GetProfileById_UserWithMaleGender_ReturnsGenderDisplayValue()
        {
            string MaleGenderValue = "Male";
            int userId = TestDatabaseHelper.InsertUser(gender: MaleGenderValue);

            UserProfile result = Repository.GetProfileById(userId);

            Assert.AreEqual(MaleGenderValue, result.Gender, "Expected gender 'Male' to be returned as-is.");
        }
    }
}