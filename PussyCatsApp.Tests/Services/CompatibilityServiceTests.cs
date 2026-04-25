using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{

    [TestClass]
    public class CompatibilityServiceTest
    {
        private Mock<IUserSkillRepository> mockUserSkillRepo;
        private Mock<ISkillGroupRepository> mockSkillGroupRepo;
        private CompatibilityService compatibilityService;
        private const int userId = 1;

        const double MinimumHighMatchScore = 50;
        [TestInitialize]
        public void Initialize()
        {
            mockUserSkillRepo = new Mock<IUserSkillRepository>();
            mockSkillGroupRepo = new Mock<ISkillGroupRepository>();
            compatibilityService = new CompatibilityService(mockUserSkillRepo.Object, mockSkillGroupRepo.Object);
        }

        [TestMethod]
        public void CalculateForRole_NoSkills_ReturnsZeroScore()
        {
            //Arrange
            mockUserSkillRepo.Setup(doesNotHaveVerifiedSkills => doesNotHaveVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }

        [TestMethod]
        public void CalculateForRole_NoGroups_ReturnsInvalidScore()
        {
            //Arrange
            mockUserSkillRepo.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(frontendRoleHasNoSkills => frontendRoleHasNoSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper))
                .Returns(new List<SkillGroup>());
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(-1, result.MatchScore);
        }


        [TestMethod]
        public void CalculateForRole_WithCvSkills_ReturnsNonZeroScore()
        {
            //Arrange
            mockUserSkillRepo.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(hasCVWithReactSkill => hasCVWithReactSkill.GetParsedCvByUserId(userId)).Returns("line1\nline2\nReact");
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.IsTrue(result.MatchScore > 0);
        }
        [TestMethod]
        public void CalculateForRole_AllSkillsVerified_ReturnsHighScore()
        {
            //Arrange
            mockUserSkillRepo.Setup(findsVerifiedSkills => findsVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 95 }
            });
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.IsTrue(result.MatchScore > MinimumHighMatchScore);
        }



        [TestMethod]
        public void CalculateForRole_CvLessThan3Lines_ReturnsZeroScore()
        {
            //Arrange
            mockUserSkillRepo.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(hasShortCV => hasShortCV.GetParsedCvByUserId(userId)).Returns("line1\nline2");
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }

        [TestMethod]
        public void CalculateForRole_CvThirdLineEmpty_ReturnsZeroScore()
        {
            //Arrange

            mockUserSkillRepo.Setup(doesNotHaveVerifiedSkills => doesNotHaveVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(hasInvalidCVLine => hasInvalidCVLine.GetParsedCvByUserId(userId)).Returns("line1\nline2\n   ");
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }


        [TestMethod]
        public void CalculateForRole_HighGroupScore_ReturnsEmptySuggestions()
        {
            //Arrange

            mockUserSkillRepo.Setup(findsVerifiedSkills => findsVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 90 }
            });
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.Suggestions.Count);
        }

        [TestMethod]
        public void CalculateForRole_MoreThan3Gaps_Returns3Suggestions()
        {
            //Arrange
            mockUserSkillRepo.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "Skill1" }, Weight = 10 },
                    new SkillGroup { GroupName = "G2", Skills = new List<string> { "Skill2" }, Weight = 9 },
                    new SkillGroup { GroupName = "G3", Skills = new List<string> { "Skill3" }, Weight = 8 },
                    new SkillGroup { GroupName = "G4", Skills = new List<string> { "Skill4" }, Weight = 7 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(userId, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(3, result.Suggestions.Count);
        }

        [TestMethod]
        public void CalculateAll_ReturnsResultForEachRole()
        {
            //Arrange
            mockUserSkillRepo.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepo.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepo.Setup(findsSkills => findsSkills.GetSkillsGroupByRole(It.IsAny<JobRole>())).Returns(new List<SkillGroup>());

            //Act
            var results = compatibilityService.CalculateAll(userId);
            //Assert
            Assert.AreEqual(Enum.GetValues(typeof(JobRole)).Length, results.Count);
        }
    }
}