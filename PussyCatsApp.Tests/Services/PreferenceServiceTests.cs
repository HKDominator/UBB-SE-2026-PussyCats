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
        private Mock<IPreferenceRepository> mockRepo;
        private PreferenceService service;

        [TestInitialize]
        public void Initialize()
        {
            mockRepo = new Mock<IPreferenceRepository>();
            service = new PreferenceService(mockRepo.Object);
        }
        [TestMethod]
        public void SavePreferences_ValidRoles_CallsRepositoryCorrectly()
        {
            // Arrange
            var roles = new List<JobRole> { JobRole.BackendDeveloper };
            var workMode = WorkMode.Remote;
            var location = "London, UK";

            // Act
            service.SavePreferences(1, roles, workMode, location);

            // Assert
            mockRepo.Verify(r => r.DeleteAllByUserId(1), Times.Once);
            mockRepo.Verify(r => r.AddPreference(It.IsAny<Preference>()), Times.Exactly(roles.Count + 2));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SavePreferences_NullRoles_ThrowsException()
        {
            // Act
            service.SavePreferences(1, null, WorkMode.Remote, "London, UK");
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
            service.SavePreferences(1, roles, WorkMode.Remote, "London, UK");
        }
        [TestMethod]
        public void SearchLocations_ValidQuery_ReturnsResults()
        {
            // Act
            var result = service.SearchLocations("London");

            // Assert
            Assert.IsTrue(result.Any(r => r.Contains("London")));
        }
        [TestMethod]
        public void SearchLocations_EmptyQuery_ReturnsEmptyList()
        {
            // Act
            var result = service.SearchLocations("");

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
