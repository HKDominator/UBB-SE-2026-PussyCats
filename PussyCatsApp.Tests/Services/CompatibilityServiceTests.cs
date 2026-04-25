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
            mockUserSkillRepository.Setup(emptySkillList => emptySkillList.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkillForGroup => roleSkillForGroup.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }

        [TestMethod]
        public void CalculateForRole_AllSkillsVerified_ReturnsHighScore()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 95 }
            });
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.IsTrue(result.MatchScore > 50);
        }


        [TestMethod]
        public void CalculateForRole_NoGroups_ReturnsInvalidScore()
        {
            //Arrange
            mockUserSkillRepository.Setup(emptySkillList => emptySkillList.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(doesNotFindCV => doesNotFindCV.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper))
                .Returns(new List<SkillGroup>());
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(-1, result.MatchScore);
        }


        [TestMethod]
        public void CalculateForRole_WithCvSkills_ReturnsNonZeroScore()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns("line1\nline2\nReact");
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.IsTrue(result.MatchScore > 0);
        }


        [TestMethod]
        public void CalculateForRole_CvLessThan3Lines_ReturnsZeroScore()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns("line1\nline2");
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }

        [TestMethod]
        public void CalculateForRole_CvThirdLineEmpty_ReturnsZeroScore()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns("line1\nline2\n   ");
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.MatchScore);
        }


        [TestMethod]
        public void CalculateForRole_HighGroupScore_ReturnsEmptySuggestions()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>
            {
                new UserSkill { SkillName = "React", IsVerified = true, Score = 90 }
            });
            mockUserSkillRepository.Setup(emptyCvForUser => emptyCvForUser.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "React" }, Weight = 10 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(0, result.Suggestions.Count);
        }

        [TestMethod]
        public void CalculateForRole_MoreThan3Gaps_Returns3Suggestions()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(JobRole.FrontendDeveloper)).Returns(
                new List<SkillGroup>
                {
                    new SkillGroup { GroupName = "G1", Skills = new List<string> { "Skill1" }, Weight = 10 },
                    new SkillGroup { GroupName = "G2", Skills = new List<string> { "Skill2" }, Weight = 9 },
                    new SkillGroup { GroupName = "G3", Skills = new List<string> { "Skill3" }, Weight = 8 },
                    new SkillGroup { GroupName = "G4", Skills = new List<string> { "Skill4" }, Weight = 7 }
                });
            //Act
            var result = compatibilityService.CalculateForRole(1, JobRole.FrontendDeveloper);
            //Assert
            Assert.AreEqual(3, result.Suggestions.Count);
        }

        [TestMethod]
        public void CalculateAll_ReturnsResultForEachRole()
        {
            //Arrange
            mockUserSkillRepository.Setup(verifiedSkills => verifiedSkills.GetVerifiedSkillsByUserId(1)).Returns(new List<UserSkill>());
            mockUserSkillRepository.Setup(parsedCv => parsedCv.GetParsedCvByUserId(1)).Returns(string.Empty);
            mockSkillGroupRepository.Setup(roleSkills => roleSkills.GetSkillsGroupByRole(It.IsAny<JobRole>())).Returns(new List<SkillGroup>());
            //Act
            var results = compatibilityService.CalculateAll(1);
            //Assert
            Assert.AreEqual(Enum.GetValues(typeof(JobRole)).Length, results.Count);
        }
    }
}