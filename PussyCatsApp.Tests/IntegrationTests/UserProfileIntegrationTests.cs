using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.Tests.Infrastructure;
using PussyCatsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Tests.IntegrationTests
{
    [TestClass]
    public class UserProfileIntegrationTests
    {
        
        private IUserProfileRepository userProfileRepository;
        private IUserProfileService userProfileService;
        private UserProfileViewModel userProfileViewModel;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.ClearAllTables();
            userProfileRepository = new UserProfileRepository(TestDatabaseHelper.ConnectionString);
            userProfileService = new UserProfileService(null, userProfileRepository);
            userProfileViewModel = new UserProfileViewModel(userProfileService, new ImageStorageService(), new CompletenessService());
        }

        [TestMethod]
        [DataRow("ACTIVE",true)]
        [DataRow("INACTIVE",false)]
        public void ToggleAccountStatusCommand_ExistingUser_TogglesCorrectly(string initialAccountStatus, bool FinalAccountStatus)
        {
            int userId = TestDatabaseHelper.InsertUser();
            Assert.IsFalse(userProfileRepository.GetProfileById(userId) == null);
            userProfileRepository.UpdateAccountStatus(userId, initialAccountStatus);

            userProfileViewModel.ToggleAccountStatusCommand();

            UserProfile userWithGivenId = userProfileService.GetProfile(userId);
            Assert.AreEqual(FinalAccountStatus, userWithGivenId.ActiveAccount);
        }

        [TestMethod]
        public async Task UploadAvatarCommand_ValidImage_UpdatesUserProfileWithNewAvatar()
        {
            // Arrange
            int testUserId = TestDatabaseHelper.InsertUser();

            await userProfileViewModel.LoadUserAsync(testUserId);

            string temporaryFolder = Path.GetTempPath();
            string temporaryImageFile = Path.Combine(temporaryFolder, $"{Guid.NewGuid()}.png");
            byte[] fakeImageRandomBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // fake bytes for the file
            File.WriteAllBytes(temporaryImageFile, fakeImageRandomBytes);

            try
            {
                using (var fileStream = File.OpenRead(temporaryImageFile))
                {
                    userProfileViewModel.UploadAvatarCommand(fileStream, Path.GetFileName(temporaryImageFile));
                }

                UserProfile userProfileFromDatabase = userProfileRepository.GetProfileById(testUserId);
                Assert.AreEqual(userProfileViewModel.UserProfile.ProfilePicture, userProfileFromDatabase.ProfilePicture,
                    "The path in the Database should match what the ViewModel holds.");
            }
            finally
            {
                if (File.Exists(userProfileViewModel.UserProfile?.ProfilePicture))
                    File.Delete(userProfileViewModel.UserProfile.ProfilePicture);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up any test data from the database if necessary
        }
    }
}
