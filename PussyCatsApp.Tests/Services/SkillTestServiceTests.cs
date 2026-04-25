using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class SkillTestServiceTests
    {

        private Mock<ISkillTestRepository> mockRepo;
        private SkillTestService service;

        [TestInitialize]
        public void Initialize()
        {
            mockRepo = new Mock<ISkillTestRepository>();
            service = new SkillTestService(mockRepo.Object);
        }
       
        [TestMethod]
        public void CanRetakeTest_ValidSkillId_ReturnsTrue()
        {
            //Arrange
            var skill = new SkillTest(1, 10, "Test1");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));
            mockRepo.Setup(skillForGivenUser => skillForGivenUser.Load(1)).Returns(skill);
            //Act
            var result = service.CanRetakeTest(1);
            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CanRetakeTest_InvalidSkillId_ThrowsException()
        {
            //Arrange
            mockRepo.Setup(nullSkillForGivenUser => nullSkillForGivenUser.Load(1)).Returns((SkillTest)null);
            //Act
            service.CanRetakeTest(1);
        }

        
        [TestMethod]
        public void SubmitRetake_EligibleTest_ReturnsNewBadge()
        {
            //Arrange
            var skill = new SkillTest(1, 10, "Test1");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-4));

            mockRepo.Setup(skillForUser => skillForUser.Load(1)).Returns(skill);
            //Act
            var result = service.SubmitRetake(1, 95);
            //Assert
            mockRepo.Verify(updateSkillTestScore => updateSkillTestScore.UpdateSkillTestScore(1, 95), Times.Once);
            mockRepo.Verify(updateAchievedDate => updateAchievedDate.UpdateAchievedDate(1, It.IsAny<DateOnly>()), Times.Once);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SubmitRetake_NotEligible_ThrowsException()
        {
            //Arrange
            var skill = new SkillTest(1, 10, "Test1");
            skill.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            mockRepo.Setup(skillForUser => skillForUser.Load(1)).Returns(skill);
            //Act
            service.SubmitRetake(1, 50);
        }

    }
}
