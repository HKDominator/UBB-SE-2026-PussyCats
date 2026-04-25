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
            var skillTest = new SkillTest(1, 10, "Test1");
            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(1)).Returns(skillTest);
            //Act
            var canUserRetakeSkillTest = skillTestService.CanRetakeTest(1);
            //Assert
            Assert.IsTrue(canUserRetakeSkillTest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CanRetakeTest_InvalidSkillId_ThrowsException()
        {
            //Arrange
            mockSkillTestRepository.Setup(doesNotFindSkillTest => doesNotFindSkillTest.Load(1)).Returns((SkillTest)null);
            //Act
            skillTestService.CanRetakeTest(1);
        }

        
        [TestMethod]
        public void SubmitRetake_EligibleTest_ReturnsNewBadge()
        {
            //Arrange
            var skillTest = new SkillTest(1, 10, "Test1");
            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));

            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(1)).Returns(skillTest);
            //Act
            var badgeResult = skillTestService.SubmitRetake(1, 95);
            //Assert
            mockSkillTestRepository.Verify(updateSkillTestScore => updateSkillTestScore.UpdateSkillTestScore(1, 95), Times.Once);
            mockSkillTestRepository.Verify(updateAchievedDate => updateAchievedDate.UpdateAchievedDate(1, It.IsAny<DateOnly>()), Times.Once);

            Assert.IsNotNull(badgeResult);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SubmitRetake_NotEligible_ThrowsException()
        {
            //Arrange
            var skillTest = new SkillTest(1, 10, "Test1");
            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            mockSkillTestRepository.Setup(findsSkillTest => findsSkillTest.Load(1)).Returns(skillTest);
            //Act
            skillTestService.SubmitRetake(1, 50);
        }

    }
}
