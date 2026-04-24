using PussyCatsApp.Utilities;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void GenerateRandomScore_GivenBounds_ReturnsScoreInBetweenBounds()
        {
            int lowerBound = 1, upperBound = 10;
            int randomScore = Helpers.GenerateRandomScore(lowerBound, upperBound);
            Assert.IsTrue(lowerBound <= randomScore && randomScore <= upperBound);
        }

        [TestMethod]
        public void GetFormattedNameFromJobRole_UIUX_ReturnsCorrectFormat()
        {
            Assert.AreEqual("UI/UX Designer", Helpers.GetFormattedNameFromJobRole(JobRole.UIUXDesigner));
        }

        [TestMethod]
        public void GetFormattedNameFromJobRole_AI_ReturnsCorrectFormat()
        {
            Assert.AreEqual("AI/ML Engineer", Helpers.GetFormattedNameFromJobRole(JobRole.AIMLEngineer));
        }

        [TestMethod]
        public void GetFormattedNameFromJobRole_DataAnalyst_ReturnsCorrectFormat()
        {
            Assert.AreEqual("Data Analyst", Helpers.GetFormattedNameFromJobRole(JobRole.DataAnalyst));
        }

        [TestMethod]
        public void GetFormattedNameFromJobRole_DevOps_ReturnsCorrectFormat()
        {
            Assert.AreEqual("Dev Ops Engineer", Helpers.GetFormattedNameFromJobRole(JobRole.DevOpsEngineer));
        }
    }
}
