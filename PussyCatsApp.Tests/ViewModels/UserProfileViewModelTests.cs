using Moq;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class UserProfileViewModelTests
    {
        private Mock<IUserProfileService> mockUserProfileService;
        private Mock<IImageStorageService> mockImageStorageService;
        private Mock<ICompletenessService> mockCompletenessService;
        private UserProfileViewModel viewModel;

        private const int testUserId = 1;
        private string takePersonalityTestText = "TAKE PERSONALITY TEST";
        private string retakePersonalityTestText = "RETAKE PERSONALITY TEST";

        [TestInitialize]
        public void Setup()
        {
            mockUserProfileService = new Mock<IUserProfileService>();
            mockImageStorageService = new Mock<IImageStorageService>();
            mockCompletenessService = new Mock<ICompletenessService>();

            viewModel = new UserProfileViewModel(mockUserProfileService.Object, mockImageStorageService.Object, mockCompletenessService.Object);
        }

        [TestMethod]
        public async Task LoadUserAsync_SetsCorrectFreshnessText()
        {
            int numberOfDaysAgo = 5;
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                LastUpdated = DateTime.Now.AddDays(-numberOfDaysAgo) // Simulate last update was some days ago
            };
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(serviceForUserProfile => serviceForUserProfile.GetProfile(testUserId)).Returns(
                userProfile
            );

            await viewModel.LoadUserAsync(testUserId);
            string expectedAnswer = TimeFormatter.CalculateFreshnessLabel(userProfile.LastUpdated);

            Assert.AreEqual(expectedAnswer, viewModel.FreshnessText);
        }

        [TestMethod]
        public async Task LoadUserAsync_SetsCorrectCompletenessPercentage_WhenProfileIsNotNull()
        {
            int numberOfDaysAgo = 5;
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                LastUpdated = DateTime.Now.AddDays(-numberOfDaysAgo) // Simulate last update was some days ago
            };
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(serviceForUserProfile => serviceForUserProfile.GetProfile(testUserId)).Returns(
                userProfile
            );

            int expectedPercentage = 50;
            mockCompletenessService.Setup(serviceForCompleteness => serviceForCompleteness.CalculateCompleteness(userProfile)).Returns(expectedPercentage);

            await viewModel.LoadUserAsync(testUserId);

            Assert.AreEqual(expectedPercentage, viewModel.CompletenessPercentage);
        }

        [TestMethod]
        public async Task LoadUserAsync_DoesNothing_WhenProfileIsNull()
        {
            UserProfile userProfile = null;
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(serviceForUserProfile => serviceForUserProfile.GetProfile(testUserId)).Returns(
                userProfile
            );

            await viewModel.LoadUserAsync(testUserId);
            
            int expectedPercentage = 0;
            Assert.AreEqual(string.Empty, viewModel.FreshnessText);
            Assert.AreEqual(expectedPercentage, viewModel.CompletenessPercentage);
            Assert.AreEqual(string.Empty, viewModel.NextEmptyFieldPrompt);
        }

        [TestMethod]
        public void ToggleAccountStatusCommand_ActivatesAccount()
        {
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                ActiveAccount = false
            };
            viewModel.UserProfile = userProfile;

            viewModel.ToggleAccountStatusCommand();
            Assert.IsTrue(viewModel.UserProfile.ActiveAccount);
        }

        [TestMethod]
        public void ToggleAccountStatusCommand_DeactivatesAccount()
        {
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                ActiveAccount = true
            };
            viewModel.UserProfile = userProfile;

            viewModel.ToggleAccountStatusCommand();
            Assert.IsFalse(viewModel.UserProfile.ActiveAccount);
        }

        [TestMethod]
        public void RemoveAvatarCommand_RemovesProfilePicture()
        {
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                ProfilePicture = "picture"
            };
            viewModel.UserProfile = userProfile;
            viewModel.RemoveAvatarCommand();

            Assert.IsNull(viewModel.UserProfile.ProfilePicture);
        }

        [TestMethod]
        public void GetPersonalityButtonText_ReturnsRetakeText_WhenUserIsNull()
        {
            viewModel.UserProfile = null;
            Assert.AreEqual(retakePersonalityTestText, viewModel.GetPersonalityButtonText());
        }

        [TestMethod]
        public void GetPersonalityButtonText_ReturnsTakeText_WhenResultIsEmpty()
        {
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                PersonalityTestResult = string.Empty
            };
            viewModel.UserProfile = userProfile;
            Assert.AreEqual(takePersonalityTestText, viewModel.GetPersonalityButtonText());
        }

        [TestMethod]
        public void GetPersonalityButtonText_ReturnsRetakeText_WhenRetakeAvailable()
        {
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                PersonalityTestResult = "Already done"
            };
            viewModel.UserProfile = userProfile;
            Assert.AreEqual(retakePersonalityTestText, viewModel.GetPersonalityButtonText());
        }

        [TestMethod]
        public void RecalculateLevelCommand_DoesNothing_WhenUserIsNull()
        {
            int oldExperiencePoints = viewModel.TotalExperiencePoints;
            viewModel.UserProfile = null;
            viewModel.RecalculateLevelCommand();
            Assert.AreEqual(oldExperiencePoints, viewModel.TotalExperiencePoints);
        }

        [TestMethod]
        public void RecalculateLevelCommand_SetsCorrectExperiencePoints()
        {
            int oldExperiencePoints = 1500, newExperiencePoints = 2000;
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                TotalExperiencePoints = oldExperiencePoints
            };
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(serviceForUserProfile => serviceForUserProfile.RecalculateLevel(userProfile)).Returns(newExperiencePoints);

            viewModel.RecalculateLevelCommand();
            Assert.AreEqual(newExperiencePoints, viewModel.TotalExperiencePoints);
        }
    }
}
