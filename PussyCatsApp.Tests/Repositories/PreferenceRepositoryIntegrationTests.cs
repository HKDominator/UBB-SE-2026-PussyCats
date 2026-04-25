using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Repositories;
using PussyCatsApp.Models;
using PussyCatsApp.Tests.Infrastructure;
using System.Collections.Generic;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class PreferenceRepositoryIntegrationTests
    {
        private PreferenceRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new PreferenceRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetPreferencesByUserId_UserHasNoPreferences_ExpectsZeroPreferences()
        {
            int ExpectedZeroPreferences = 0;
            int userId = TestDatabaseHelper.InsertUser();

            List<Preference> result = Repository.GetPreferencesByUserId(userId);

            Assert.AreEqual(ExpectedZeroPreferences, result.Count);
        }

        [TestMethod]
        public void GetPreferencesByUserId_UserHasTwoPreferences_ExpectsTwoPreferences()
        {
            int ExpectedTwoPreferences = 2;
            string ThemePreferenceType = "Theme";
            string ThemeValue = "Dark";
            string NotificationPreferenceType = "Notifications";
            string NotificationValue = "Enabled";

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertPreference(userId, ThemePreferenceType, ThemeValue);
            TestDatabaseHelper.InsertPreference(userId, NotificationPreferenceType, NotificationValue);

            List<Preference> result = Repository.GetPreferencesByUserId(userId);

            Assert.AreEqual(ExpectedTwoPreferences, result.Count);
        }

        [TestMethod]
        public void AddPreference_ValidPreference_SavedToDatabase()
        {
            string LanguagePreferenceType = "Language";
            string LanguageValue = "English";
            int FirstIndex = 0;

            int userId = TestDatabaseHelper.InsertUser();
            Preference newPreference = new Preference
            {
                UserId = userId,
                PreferenceType = LanguagePreferenceType,
                Value = LanguageValue
            };

            Repository.AddPreference(newPreference);

            List<Preference> result = Repository.GetPreferencesByUserId(userId);
            Assert.AreEqual(LanguagePreferenceType, result[FirstIndex].PreferenceType);
        }

        [TestMethod]
        public void RemovePreference_PreferenceExists_ExpectsOnlyThatPreferenceRemoved()
        {
            string PreferenceTypeA = "A";
            string PreferenceValue1 = "1";
            string PreferenceTypeB = "B";
            string PreferenceValue2 = "2";
            int FirstIndex = 0;

            int userId = TestDatabaseHelper.InsertUser();
            int preferenceId1 = TestDatabaseHelper.InsertPreference(userId, PreferenceTypeA, PreferenceValue1);
            int preferenceId2 = TestDatabaseHelper.InsertPreference(userId, PreferenceTypeB, PreferenceValue2);

            Repository.RemovePreference(preferenceId1);

            List<Preference> result = Repository.GetPreferencesByUserId(userId);
            Assert.AreEqual(PreferenceTypeB, result[FirstIndex].PreferenceType, "The wrong preference was deleted.");
        }

        [TestMethod]
        public void DeleteAllByUserId_UserHasMultiplePreferences_ExpectsAllPreferencesClearedForThatUser()
        {
            string UserEmail = "user1@test.com";
            string ColorPreferenceType = "Color";
            string ColorValue = "Red";
            string FontPreferenceType = "Font";
            string FontValue = "Arial";
            int ExpectedCountAfterDelete = 0;

            int userId1 = TestDatabaseHelper.InsertUser(email: UserEmail);

            TestDatabaseHelper.InsertPreference(userId1, ColorPreferenceType, ColorValue);
            TestDatabaseHelper.InsertPreference(userId1, FontPreferenceType, FontValue);

            Repository.DeleteAllByUserId(userId1);

            int resultCount = Repository.GetPreferencesByUserId(userId1).Count;
            Assert.AreEqual(ExpectedCountAfterDelete, resultCount, "User 1 should have 0 preferences.");
        }
    }
}