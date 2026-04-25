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
            int ExpectedZeroSkills = 0;
            int userId = TestDatabaseHelper.InsertUser();

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual(ExpectedZeroSkills, result.Count);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_UserHasOneSkill_ExpectsOneSkill()
        {
            int ExpectedOneSkill = 1;
            int SkillScore = 90;
            string SkillName = "C#";

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, SkillName, SkillScore);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual(ExpectedOneSkill, result.Count);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_SkillExists_ExpectsCorrectSkillName()
        {
            string ExpectedSkillName = "SQL";
            int SkillScore = 80;
            int FirstIndex = 0;

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, ExpectedSkillName, SkillScore);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.AreEqual(ExpectedSkillName, result[FirstIndex].SkillName);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_SkillExists_ExpectsSkillVerified()
        {
            string SkillName = "Git";
            int SkillScore = 50;
            int FirstIndex = 0;

            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertSkill(userId, SkillName, SkillScore);

            var result = Repository.GetVerifiedSkillsByUserId(userId);

            Assert.IsTrue(result[FirstIndex].IsVerified);
        }

        [TestMethod]
        public void GetVerifiedSkillsByUserId_MultipleUsers_ExpectsOnlyTargetUserSkills()
        {
            string TargetEmail = "target@test.com";
            string OtherEmail = "other@test.com";
            string TargetSkillName = "TargetSkill";
            string OtherSkillName = "OtherSkill";
            int DefaultScore = 10;

            int targetUser = TestDatabaseHelper.InsertUser(email: TargetEmail);
            int otherUser = TestDatabaseHelper.InsertUser(email: OtherEmail);

            TestDatabaseHelper.InsertSkill(targetUser, TargetSkillName, DefaultScore);
            TestDatabaseHelper.InsertSkill(otherUser, OtherSkillName, DefaultScore);

            var result = Repository.GetVerifiedSkillsByUserId(targetUser);

            Assert.IsFalse(result.Any(skill => skill.SkillName == OtherSkillName));
        }

        [TestMethod]
        public void GetParsedCvByUserId_UserDoesNotExist_ExpectsNull()
        {
            int NonExistentUserId = 9999;

            var result = Repository.GetParsedCvByUserId(NonExistentUserId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParsedCvByUserId_CvFieldIsNull_ExpectsNull()
        {
            string NullCurriculumVitae = null;
            int userId = TestDatabaseHelper.InsertUser(parsedCv: NullCurriculumVitae);

            var result = Repository.GetParsedCvByUserId(userId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParsedCvByUserId_CvHasValue_ExpectsCorrectContent()
        {
            string ExpectedCurriculumVitaeContent = "Experience with .NET and SQL";
            int userId = TestDatabaseHelper.InsertUser(parsedCv: ExpectedCurriculumVitaeContent);

            var result = Repository.GetParsedCvByUserId(userId);

            Assert.AreEqual(ExpectedCurriculumVitaeContent, result);
        }
    }
}