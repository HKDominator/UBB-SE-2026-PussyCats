using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class BadgeTests
    {
        [TestMethod]
        [DataRow(95f, BadgeTier.GOLD)]
        [DataRow(90f, BadgeTier.GOLD)]
        [DataRow(89f, BadgeTier.SILVER)]
        [DataRow(70, BadgeTier.SILVER)]
        [DataRow(69.6f, BadgeTier.BRONZE)]
        [DataRow(50f, BadgeTier.BRONZE)]
        [DataRow(49f, BadgeTier.PARTICIPANT)]
        [DataRow(0f, BadgeTier.PARTICIPANT)]
        [DataRow(-23f, BadgeTier.PARTICIPANT)]
        public void AssignTier_GivenScore_ReturnsCorrectTier(float score, BadgeTier tier)
        {
            var returnedBadge = SimpleModelOperations.AssignTier(score);

            Assert.AreEqual(tier, returnedBadge.Tier);
        }

        [TestMethod]
        [DataRow(95f, 100)]
        [DataRow(90f, 100)]
        [DataRow(89f, 60)]
        [DataRow(70f, 60)]
        [DataRow(69.9f, 30)]
        [DataRow(50f, 30)]
        [DataRow(49f, 10)]
        public void AssignTier_GivenScore_ReturnsCorrectExperiencePoints(float score, int expectedXp)
        {
            var returnedBadge = SimpleModelOperations.AssignTier(score);

            Assert.AreEqual(expectedXp, returnedBadge.XpValue);

        }
    }
}
