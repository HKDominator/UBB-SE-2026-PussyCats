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
        private Mock<IUserSkillRepository> mockUserSkillRepository;
        private Mock<ISkillGroupRepository> mockSkillGroupRepository;
        private CompatibilityService compatibilityService;
        

        const double MinimumHighMatchScore = 50;
        [TestInitialize]
        public void Initialize()
        {
            mockUserSkillRepository = new Mock<IUserSkillRepository>();
            mockSkillGroupRepository = new Mock<ISkillGroupRepository>();
            compatibilityService = new CompatibilityService(mockUserSkillRepository.Object, mockSkillGroupRepository.Object);
        }

        [TestMethod]
        public void CalculateForRole_NoSkills_ReturnsZeroScore()
        {
            //Arrange
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotHaveVerifiedSkills => doesNotHaveVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
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
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(frontendRoleHasNoSkills => frontendRoleHasNoSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper))
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
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(hasCVWithReactSkill => hasCVWithReactSkill.GetParsedCvByUserId(userId)).Returns("line1\nline2\nReact");
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
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
            int userId = 1;
            mockUserSkillRepository.Setup(findsVerifiedSkills => findsVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 95 }
            });
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

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
            int userId = 1;
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(hasShortCV => hasShortCV.GetParsedCvByUserId(userId)).Returns("line1\nline2");
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

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
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotHaveVerifiedSkills => doesNotHaveVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(hasInvalidCVLine => hasInvalidCVLine.GetParsedCvByUserId(userId)).Returns("line1\nline2\n   ");
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

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
            int userId = 1;
            mockUserSkillRepository.Setup(findsVerifiedSkills => findsVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 90 }
            });
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

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
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(frontendRoleHasSkills => frontendRoleHasSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(

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
            int userId = 1;
            mockUserSkillRepository.Setup(doesNotFindVerifiedSkills => doesNotFindVerifiedSkills.GetVerifiedSkillsByUserId(userId)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(userId)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(findsSkills => findsSkills.GetSkillsGroupByRole(It.IsAny<JobRole>())).Returns(new List<SkillGroup>());

            //Act
            var results = compatibilityService.CalculateAll(userId);
            //Assert
            Assert.AreEqual(Enum.GetValues(typeof(JobRole)).Length, results.Count);
        }
    }
}