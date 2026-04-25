using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class SkillTestRepositoryIntegrationTests
    {
        private SkillTestRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new SkillTestRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void Load_ExistingSkill_ExpectsCorrectSkillName()
        {
            const string SkillName = "C# Programming";
            const int InitialScore = 5;
            DateTime now = DateTime.Now;

            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, SkillName, InitialScore, now);

            SkillTest resultFromDb = Repository.Load(skillId);

            Assert.AreEqual(SkillName, resultFromDb.Name);
        }

        [TestMethod]
        public void Save_UpdateUserSkill_ExpectsNewSkillReturn()
        {
            const string SkillName = "SQL";
            const int OriginalScore = 10;
            const int UpdatedScore = 100;
            const int Year = 2026;
            const int Month = 4;
            const int Day = 21;

            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, SkillName, OriginalScore, DateTime.Now);

            DateOnly achievedDate = new DateOnly(Year, Month, Day);
            SkillTest newSkill = new SkillTest(skillId, userId, SkillName, UpdatedScore, achievedDate);

            Repository.Save(skillId, newSkill);
            SkillTest updated = Repository.Load(skillId);

            Assert.AreEqual(UpdatedScore, updated.Score);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_UserHasTwoTests_ExpectsCorrectCountOfTwo()
        {
            const int ExpectedCount = 2;
            const int ScoreOne = 60;
            const int ScoreTwo = 70;

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, "First test", ScoreOne, DateTime.Now);
            TestDatabaseHelper.InsertSkill(userId, "Second test", ScoreTwo, DateTime.Now);

            List<SkillTest> result = Repository.GetSkillTestsByUserId(userId);

            Assert.AreEqual(ExpectedCount, result.Count());
        }

        [TestMethod]
        public void UpdateSkillTestScore_UserHasNewScore_ExpectsTheNewScore()
        {
            const int InitialScore = 20;
            const int FinalScore = 80;

            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, "Java", InitialScore, DateTime.Now);

            Repository.UpdateSkillTestScore(skillId, FinalScore);

            SkillTest result = Repository.Load(skillId);

            Assert.AreEqual(FinalScore, result.Score);
        }

        [TestMethod]
        public void UpdateAchivedDate_UserHasNewDateForTest_ExpectsNewDate()
        {
            const int Year = 2026;
            const int Month = 4;
            const int Day = 21;
            const int SkillScore = 65;

            int userId = TestDatabaseHelper.InsertUser();
            int skillId = TestDatabaseHelper.InsertSkill(userId, "SQL", SkillScore, DateTime.Now);
            DateOnly newDate = new DateOnly(Year, Month, Day);

            Repository.UpdateAchievedDate(skillId, newDate);

            SkillTest result = Repository.Load(skillId);

            Assert.AreEqual(newDate, result.AchievedDate);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Load_SkillDoesNotExist_ExpectsThrownException()
        {
            const int NonExistentId = 1297;
            Repository.Load(NonExistentId);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_InvalidServer_ExpectsEmptyList()
        {
            const int DummyUserId = 1;
            const int ExpectedZero = 0;
            string badConn = "Server=FakeServer;Database=FakeDB;Connect Timeout=1;";

            var badRepo = new SkillTestRepository(badConn);
            List<SkillTest> result = badRepo.GetSkillTestsByUserId(DummyUserId);

            Assert.AreEqual(ExpectedZero, result.Count);
        }

        [TestMethod]
        public void Load_MalformedConnectionString_ExpectsSpecificExceptionThrown()
        {
            const int DummyId = 1;
            const string MalformedString = "This is not a connection string";
            const string ErrorFragment = "not found";

            SkillTestRepository badRepository = new SkillTestRepository(MalformedString);
            try
            {
                badRepository.Load(DummyId);
            }
            catch (Exception exception)
            {
                Assert.IsTrue(exception.Message.Contains(ErrorFragment));
            }
        }

        [TestMethod]
        public void UpdateSkillTestScore_InvalidServer_ExpectsNoCrash()
        {
            const int DummyId = 1;
            const int DummyScore = 100;
            string badConnection = "Server=FakeServer;Database=FakeDB;Connect Timeout=1;";

            var badRepository = new SkillTestRepository(badConnection);
            badRepository.UpdateSkillTestScore(DummyId, DummyScore);
        }

        [TestMethod]
        public void UpdateAchievedDate_InvalidServer_ExpectsNoCrash()
        {
            const int DummyId = 1;
            string badConnection = "Server=FakeServer;Database=FakeDB;Connect Timeout=1;";

            var badRepository = new SkillTestRepository(badConnection);
            badRepository.UpdateAchievedDate(DummyId, default);
        }

        [TestMethod]
        public void Save_InvalidServer_ExpectsNoCrash()
        {
            const int DummyId = 1;
            const int DummyUserId = 1;
            const int DummyScore = 0;
            string badConnection = "Server=FakeServer;Database=FakeDB;Connect Timeout=1;";

            var badRepository = new SkillTestRepository(badConnection);
            badRepository.Save(DummyId, new SkillTest(DummyId, DummyUserId, "Test", DummyScore, default));
        }

        [TestMethod]
        public void Load_DatabaseNotFound_ExpectsSqlError()
        {
            const int DummyId = 1;
            const string ExpectedMessage = "SkillTest with ID 1 not found.";
            string sqlExceptionConnString = "Server=ASUS\\SQLEXPRESS;Database=DB_THAT_DOES_NOT_EXIST;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=2;";

            var repositoryWithSqlError = new SkillTestRepository(sqlExceptionConnString);

            try
            {
                repositoryWithSqlError.Load(DummyId);
                Assert.Fail("The method should have thrown an Exception after catching the SqlException.");
            }
            catch (Exception exception)
            {
                Assert.AreEqual(ExpectedMessage, exception.Message);
            }
        }

        [TestMethod]
        public void Save_MalformedConnectionString_ExpectsErrorBegingCatched()
        {
            const int DummyId = 1;
            const int DummyUserId = 1;
            const int DummyScore = 0;
            string malformedString = "ThisIsNotAConnectionString";

            var repositoryWithGeneralError = new SkillTestRepository(malformedString);
            var dummyData = new SkillTest(DummyId, DummyUserId, "Test", DummyScore, default);

            repositoryWithGeneralError.Save(DummyId, dummyData);
        }

        [TestMethod]
        public void UpdateSkillTestScore_MalformedConnectionString_ExpectsErrorBegingCacthed()
        {
            const int DummyId = 1;
            const int DummyScore = 100;
            string malformedConnectionString = "Invalid Format Here";

            var repository = new SkillTestRepository(malformedConnectionString);
            repository.UpdateSkillTestScore(DummyId, DummyScore);
        }

        [TestMethod]
        public void UpdateAchievedDate_MalformedConnectionString_ExpectsErrorBegingCatched()
        {
            const int DummyId = 1;
            const int Year = 2026;
            const int Month = 1;
            const int Day = 1;
            string malformedConnectionString = "Bad;Format;No;Equal;Sign";

            var repository = new SkillTestRepository(malformedConnectionString);
            DateOnly dummyDate = new DateOnly(Year, Month, Day);

            repository.UpdateAchievedDate(DummyId, dummyDate);
        }

        [TestMethod]
        public void GetSkillTestsByUserId_EmptyConnectionString_CatchesGeneralException()
        {
            const int DummyUserId = 1;
            const int ExpectedZero = 0;
            string emptyConnectionString = "";

            var repository = new SkillTestRepository(emptyConnectionString);
            List<SkillTest> result = repository.GetSkillTestsByUserId(DummyUserId);

            Assert.AreEqual(ExpectedZero, result.Count);
        }
    }
}