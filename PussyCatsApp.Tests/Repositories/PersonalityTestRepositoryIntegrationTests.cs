using PussyCatsApp.Repositories.PersonalityTestRepo;
using PussyCatsApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class PersonalityTestRepositoryIntegrationTests
    {
        private PersonalityTestRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new PersonalityTestRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void Load_UserHasResult_ExpectsCorrectStringResult()
        {
            string expectedResult = "Expected Personality Result";
            int userId = TestDatabaseHelper.InsertUser(personalityTestResult: expectedResult);

            var resultFromDb = Repository.Load(userId);

            Assert.AreEqual(expectedResult, resultFromDb);

        }

        [TestMethod]
        public void Load_UserDoesNotExist_ExpectsNullResult()
        {
            int nonExistentId = 10876;
            var resultFromDb = Repository.Load(nonExistentId);
            Assert.IsNull(resultFromDb);
        }

        [TestMethod]
        public void Save_UpdateUsersPersonalityTestResult_ExpectesNewResult()
        {
            string beforeResult="Before Result";
            int userId=TestDatabaseHelper.InsertUser(personalityTestResult: beforeResult);
            string afterResult = "After Result";

            Repository.Save(userId, afterResult);

            string resultFromDb=TestDatabaseHelper.GetUserPersonalityTestResult(userId);

            Assert.AreEqual(afterResult, resultFromDb);
        }


        [TestMethod]
        public void Save_SaveResultForExistingUser_ExpectsNoException()
        {
            int userId = TestDatabaseHelper.InsertUser();
            string result = "Result";
            try
            {
                Repository.Save(userId, result);
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected no exception, but got: {exception.Message}");
            }
        }

        [TestMethod]
        public void Save_UpdateResultForNonExistingUser_ExpectsNoException()
        {
            string result = "Result";
            int nonExistentId = 10867;
            try
            {
                Repository.Save(nonExistentId, result);
            }
            catch (Exception exception)
            {
                Assert.Fail($"Expected no exception, but got: {exception.Message}");
            }
        }

        [TestMethod]
        public void Load_SqlException_ExpectsExceptionBeingCaught()
        {
            int idToLoad = 1;
            string deadConnectionString = "Server=NonExistentServer;Database=FakeDb;Trusted_Connection=True;Connect Timeout=1;TrustServerCertificate=True;";
            var repository = new PersonalityTestRepository(deadConnectionString);

            var result = repository.Load(idToLoad);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Save_SqlException_ExceptionBeingHandledInDebugLine()
        {
            int idToSave = 1;
            string deadConnectionString = "Server=NonExistentServer;Database=FakeDb;Trusted_Connection=True;Connect Timeout=1;TrustServerCertificate=True;";
            var repository = new PersonalityTestRepository(deadConnectionString);

            repository.Save(idToSave, "Some Result");
        }
    }
}
