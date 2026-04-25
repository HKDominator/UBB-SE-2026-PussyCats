using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class MatchRepositoryIntegrationTests
    {
        private MatchRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new MatchRepository(TestDatabaseHelper.ConnectionString);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasNoMatches_ExpectsZeroMatches()
        {
            int expectedNumberOfMatches = 0;
            int userId = TestDatabaseHelper.InsertUser();

            var matches = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(expectedNumberOfMatches, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasOneMatch_ExpectsOneMatch()
        {
            int expectedNumberOfMatches = 1;
            int userId = TestDatabaseHelper.InsertUser();
            int matchId = TestDatabaseHelper.InsertMatch(userId, "LSEG", "DevOps Engineer", new DateTime(2026, 1, 1));

            var matches = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(expectedNumberOfMatches, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserHasTwoMatches_ExpectsMatchesInDescendingOrder()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int matchId1 = TestDatabaseHelper.InsertMatch(userId, "LSEG", "DevOps Engineer", new DateTime(2026, 2, 1));
            int matchId2 = TestDatabaseHelper.InsertMatch(userId, "Bosch", "Software Engineer", new DateTime(2026, 1, 1));

            var matchesFound = Repository.GetMatchesByUserId(userId);

            Assert.AreEqual(matchId2, matchesFound[1].Id);
        }

        [TestMethod]
        public void GetMatchesByUserId_UserDoesNotExist_ExpectsZeroMatches()
        {
            int expectedNumberOfMatches = 0;

            var matches = Repository.GetMatchesByUserId(10867);

            Assert.AreEqual(expectedNumberOfMatches, matches.Count);
        }

        [TestMethod]
        public void GetMatchesByUserId_DatabaseNotAvalaible_ExpectsZeroResults()
        {
            int expectedNumberOfMatches = 0;
            string invalidConnectionString = "Server=ASUS\\SQLEXPRESS;Database=PussyCatsTestsDBNotExistient;Trusted_Connection=True;TrustServerCertificate=True;";
            var repositoryWithInvalidConnection = new MatchRepository(invalidConnectionString);

            var result = repositoryWithInvalidConnection.GetMatchesByUserId(1).Count;

            Assert.AreEqual(expectedNumberOfMatches, result);
        }

        [TestMethod]
        public void GetMatchesByUserId_MalformedConnectionString_ExpectsNullResult()
        {
            string malformedConnectionString = "ConnectionStringInvalid";
            MatchRepository invalidRepository = new MatchRepository(malformedConnectionString);

            var result = invalidRepository.GetMatchesByUserId(1);

            Assert.IsNotNull(result);
        }

    }
}
