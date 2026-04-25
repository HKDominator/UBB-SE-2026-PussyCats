using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class PreferenceServiceTests
    {
        private Mock<IPreferenceRepository> mockPreferenceRepository;
        private PreferenceService preferenceService;

        [TestInitialize]
        public void Initialize()
        {
            mockPreferenceRepository = new Mock<IPreferenceRepository>();
            preferenceService = new PreferenceService(mockPreferenceRepository.Object);
        }
        [TestMethod]
        public void SavePreferences_ValidRoles_CallsRepositoryCorrectly()
        {
            // Arrange
            var userId = 1;
            var roles = new List<JobRole> { JobRole.BackendDeveloper };
            var workMode = WorkMode.Remote;
            var location = "London, UK";

            // Act
            preferenceService.SavePreferences(userId, roles, workMode, location);

            // Assert
            mockPreferenceRepository.Verify(preferencesDeletedForUser => preferencesDeletedForUser.DeleteAllByUserId(1), Times.Once);
            mockPreferenceRepository.Verify(addPreferenceForAUser => addPreferenceForAUser.AddPreference(It.IsAny<Preference>()), Times.Exactly(roles.Count + 2));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SavePreferences_NullRoles_ThrowsException()
        {
            // Act
            preferenceService.SavePreferences(1, null, WorkMode.Remote, "London, UK");
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SavePreferences_TooManyRoles_ThrowsException()
        {
            // Arrange
            var roles = new List<JobRole>
            {
                JobRole.BackendDeveloper,
                JobRole.FrontendDeveloper,
                JobRole.DataAnalyst,
                JobRole.ProjectManager
            };

            // Act
            preferenceService.SavePreferences(1, roles, WorkMode.Remote, "London, UK");
        }
        [TestMethod]
        public void SearchLocations_ValidQuery_ReturnsResults()
        {
            // Act
            var result = preferenceService.SearchLocations("London");

            // Assert
            Assert.IsTrue(result.Any(userLocationsList => userLocationsList.Contains("London")));
        }
        [TestMethod]
        public void SearchLocations_EmptyQuery_ReturnsEmptyList()
        {
            // Act
            var result = preferenceService.SearchLocations("");

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
