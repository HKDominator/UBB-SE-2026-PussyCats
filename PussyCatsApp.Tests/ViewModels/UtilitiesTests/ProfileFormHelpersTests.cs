using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class ProfileFormHelpersTests
    {
        [TestMethod]
        public void UniversityMatchesAllWords_AllWordsMatch_ReturnsTrue()
        {
            string university = "Massachusetts Institute of Technology";
            string[] words = new string[] { "massachusetts", "institute", "technology" };
            bool result = ProfileFormHelpers.UniversityMatchesAllWords(university, words);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UniversityMatchesAllWords_AtLeastOneWordDoesNotMatch_ReturnsFalse()
        {
            string university = "Massachusetts Institute of Technology";
            string[] words = new string[] { "massachusetts", "institute", "harvard" };
            bool result = ProfileFormHelpers.UniversityMatchesAllWords(university, words);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FilterUniversitiesHelper_MatchesExist_ReturnsMatchingUniversity()
        {
            string universityQuery = "Babes University";
            List<string> results = ProfileFormHelpers.FilterUniversitiesHelper(universityQuery);
            Assert.IsTrue(results.Contains("Babes-Bolyai University"));
        }

        [TestMethod]
        public void FilterUniversitiesHelper_EmptyQuery_ReturnsEmptyList()
        {
            string universityQuery = string.Empty;
            List<string> results = ProfileFormHelpers.FilterUniversitiesHelper(universityQuery);
            const int expectedResultsCount = 0;
            Assert.AreEqual(expectedResultsCount, results.Count);
        }

        [TestMethod]
        public void FilterUniversitiesHelper_NoMatches_ReturnsEmptyList()
        {
            string universityQuery = "Nonexistent University";
            List<string> results = ProfileFormHelpers.FilterUniversitiesHelper(universityQuery);
            const int expectedResultsCount = 0;
            Assert.AreEqual(expectedResultsCount, results.Count);
        }
    }
}
