using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class TimeFormatterTests
    {
        [TestMethod]
        public void CalculateFreshnessLabel_UpdatedToday_ReturnsCorrectFormatted()
        {
            DateTime targetDateToday = DateTime.Now.Date;
            Assert.AreEqual("Profile last updated: Today", TimeFormatter.CalculateFreshnessLabel(targetDateToday));
        }

        [TestMethod]
        public void CalculateFreshnessLabel_Updated_Yesterday_ReturnsCorrectFormatted()
        {
            DateTime targetDateYesterday = DateTime.Now.Date.AddDays(-1);
            Assert.AreEqual("Profile last updated: Yesterday", TimeFormatter.CalculateFreshnessLabel(targetDateYesterday));
        }

        [TestMethod]
        public void CalculateFreshnessLabel_UpdatedMultipleDaysAgo_ReturnsCorrectFormatted()
        {
            int numberOfDays = 5;
            DateTime targetDateMultipleDaysAgo = DateTime.Now.Date.AddDays(-numberOfDays);
            Assert.AreEqual($"Profile last updated: {numberOfDays} days ago", TimeFormatter.CalculateFreshnessLabel(targetDateMultipleDaysAgo));
        }
    }
}
