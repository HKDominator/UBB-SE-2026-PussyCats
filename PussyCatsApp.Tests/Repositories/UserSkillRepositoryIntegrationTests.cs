using Microsoft.VisualStudio.TestTools.UnitTesting;
using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System.Linq;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class UserSkillRepositoryIntegrationTests
    {
        private UserSkillRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new UserSkillRepository(TestDatabaseHelper.ConnectionString);
        }


        [TestMethod]
        public void GetVerifiedSkillsByUserId_UserHasNoSkills_ExpectsZeroSkills()
        {
            int userId = TestDatabaseHelper.InsertUser();

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_UserHasOneSkill_ExpectsOneSkill()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, "C#", 90);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_SkillExists_ExpectsCorrectSkillName()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, "SQL", 80);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual("SQL", result[0].SkillName);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_SkillExists_ExpectsSkillVerified()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, "Git", 50);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.IsTrue(result[0].IsVerified);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_MultipleUsers_ExpectsOnlyTargetUserSkills()
        {
            int targetUser = TestDatabaseHelper.InsertUser(email: "target@test.com");
            int otherUser = TestDatabaseHelper.InsertUser(email: "other@test.com");
            TestDatabaseHelper.InsertSkill(targetUser, "TargetSkill", 10);
            TestDatabaseHelper.InsertSkill(otherUser, "OtherSkill", 10);

            var result = Repository.GetVerifiedSkillsByUserId(targetUser);

            Assert.IsFalse(result.Any(NotSpecifiedSkill => NotSpecifiedSkill.SkillName == "OtherSkill"));
        }


        [TestMethod]
        public void GetParsedCvByUserId_UserDoesNotExist_ExpectsNull()
        {
            var result = Repository.GetParsedCvByUserId(9999);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParsedCvByUserId_CvFieldIsNull_ExpectsNull()
        {
            int userId = TestDatabaseHelper.InsertUser(parsedCv: null);
            var result = Repository.GetParsedCvByUserId(userId);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParsedCvByUserId_CvHasValue_ExpectsCorrectContent()
        {
            string expectedCv = "Experience with .NET and SQL";
            int userId = TestDatabaseHelper.InsertUser(parsedCv: expectedCv);

            var result = Repository.GetParsedCvByUserId(userId);

            Assert.AreEqual(expectedCv, result);
        }
    }
}