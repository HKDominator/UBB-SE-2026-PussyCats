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

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up any test data from the database if necessary
        }
    }
}
