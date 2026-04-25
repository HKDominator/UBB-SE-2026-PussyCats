using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class UserLevelTests
    {
        [TestMethod]
        [DataRow(900, 5, UserLevel.UserTitle.Expert, 800, 0)]
        [DataRow(800, 5, UserLevel.UserTitle.Expert, 800, 0)]
        [DataRow(799, 4, UserLevel.UserTitle.Specialist, 500, 800)]
        [DataRow(500, 4, UserLevel.UserTitle.Specialist, 500, 800)]
        [DataRow(499, 3, UserLevel.UserTitle.Practitioner, 250, 500)]
        [DataRow(250, 3, UserLevel.UserTitle.Practitioner, 250, 500)]
        [DataRow(249, 2, UserLevel.UserTitle.Apprentice, 100, 250)]
        [DataRow(100, 2, UserLevel.UserTitle.Apprentice, 100, 250)]
        [DataRow(99, 1, UserLevel.UserTitle.Newcomer, 0, 100)]
        [DataRow(0, 1, UserLevel.UserTitle.Newcomer, 0, 100)]
        public void CalculateLevel_GivenPositiveXp_ReturnsCorrectLevel(
            int xp,
            int expectedLevel,
            UserLevel.UserTitle expectedTitle,
            int expectedMinXp,
            int expectedMaxXp)
        {
            var level = UserLevelService.CalculateLevel(xp);

            Assert.AreEqual(expectedLevel, level.LevelNumber);
            Assert.AreEqual(expectedTitle, level.Title);
            Assert.AreEqual(expectedMinXp, level.XpRequired);
            Assert.AreEqual(expectedMaxXp, level.NextLevelXp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalculateLevel_NegativeXp_ThrowsArgumentException()
        {
            var level = UserLevelService.CalculateLevel(-1);
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(27, 27)]
        [DataRow(100, 0)]
        [DataRow(175, 50)]
        [DataRow(250, 0)]
        [DataRow(500, 0)]
        [DataRow(503, 1)]
        [DataRow(799, 99)]
        [DataRow(800, 100)]
        [DataRow(900, 100)]
        public void GetLevelProgressPercent_GivenTotalPositiveXp_ReturnsCorrectProgress(int givenXp, int expectedPercentage)
        {
            UserLevel level = UserLevelService.CalculateLevel(givenXp);

            int progressPercent = UserLevelService.GetLevelProgressPercent(givenXp, level);

            Assert.AreEqual(expectedPercentage, progressPercent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetLevelProgressPercent_GivenNegativeXp_ThrowsArgumentException()
        {
            UserLevel level = new UserLevel();
            UserLevelService.GetLevelProgressPercent(-23, level);
        }

        [TestMethod]
        [DataRow(0, 100)]
        [DataRow(27, 73)]
        [DataRow(100, 150)]
        [DataRow(249,1)]
        [DataRow(250, 250)]
        [DataRow(500, 300)]
        [DataRow(799, 1)]
        public void GetXpToNextLevel_GivenTotalXpIn0To799Range_ReturnsCorrectXpToNextLevel(int givenXp, int expectedNrXpToNextLevel)
        {
            var level = UserLevelService.CalculateLevel(175);
            int xpToNextLevel = UserLevelService.GetXpToNextLevel(175,level);
            Assert.AreEqual(75, xpToNextLevel);
        }

        [TestMethod]
        [DataRow(800,0)]
        [DataRow(900,0)]
        public void GetXpToNextLevel_GivenTotalXpAbove800_ReturnsCorrectXpToNextLevel(int givenXp, int expectedNrXpToNextLevel)
        {
            var level = UserLevelService.CalculateLevel(175);
            int xpToNextLevel = UserLevelService.GetXpToNextLevel(175,level);
            Assert.AreEqual(75, xpToNextLevel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetXpToNextLevel_GivenTotalXpNegative_ThrowsArgumentException()
        {
            var level = UserLevelService.CalculateLevel(-10);
            
        }
    }
}
