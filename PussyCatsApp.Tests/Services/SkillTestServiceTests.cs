using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class SkillTestServiceTests
    {

        private Mock<ISkillTestRepository> mockSkillTestRepository;
        private SkillTestService skillTestService;
        private const int skillTestId = 1;
        private const int ExcellentRetakeScore = 95;
        private const int FailingRetakeScore = 50;

        [TestInitialize]
        public void Initialize()
        {
            mockSkillTestRepository = new Mock<ISkillTestRepository>();
            skillTestService = new SkillTestService(mockSkillTestRepository.Object);
        }
       
        [TestMethod]
        public void CanRetakeTest_ValidSkillId_ReturnsTrue()
        {
            //Arrange
            var skillTest = new SkillTest(skillTestId, 10, "Test1");
            DateOnly fourMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
            skillTest.AchievedDate = fourMonthsAgo;
            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(skillTestId)).Returns(skillTest);
            //Act
            var canUserRetakeSkillTest = skillTestService.CanRetakeTest(skillTestId);
            //Assert
            Assert.IsTrue(canUserRetakeSkillTest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CanRetakeTest_InvalidSkillId_ThrowsException()
        {
            //Arrange
            mockSkillTestRepository.Setup(doesNotFindSkillTest => doesNotFindSkillTest.Load(skillTestId)).Returns((SkillTest)null);
            //Act
            skillTestService.CanRetakeTest(skillTestId);
        }

        
        [TestMethod]
        public void SubmitRetake_EligibleTest_ReturnsNewBadge()
        {
            //Arrange
            var skillTest = new SkillTest(skillTestId, 10, "Test1");
            DateOnly fourMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
            skillTest.AchievedDate = fourMonthsAgo;

            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(skillTestId)).Returns(skillTest);
            //Act
            var badgeResult = skillTestService.SubmitRetake(skillTestId, ExcellentRetakeScore);
            //Assert
            mockSkillTestRepository.Verify(updateSkillTestScore => updateSkillTestScore.UpdateSkillTestScore(skillTestId, ExcellentRetakeScore), Times.Once);
            mockSkillTestRepository.Verify(updateAchievedDate => updateAchievedDate.UpdateAchievedDate(skillTestId, It.IsAny<DateOnly>()), Times.Once);

            Assert.IsNotNull(badgeResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SubmitRetake_NotEligible_ThrowsException()
        {
            //Arrange
            var skillTest = new SkillTest(skillTestId, 10, "Test1");
            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(skillTestId)).Returns(skillTest);
            //Act
            skillTestService.SubmitRetake(skillTestId, FailingRetakeScore);
        }

    }
}
