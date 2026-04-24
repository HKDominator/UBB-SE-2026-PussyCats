using PussyCatsApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Tests.IntegrationTests
{
    [TestClass]
    public class UserProfileIntegrationTests
    {
        
        private IUserProfileRepository userProfileRepository;
        private IUserProfileService userProfileService;

        [TestInitialize]
        public void Setup()
        {
            userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());// TO DO : move to test database
            userProfileService = new UserProfileService(null, userProfileRepository);

        }

        [TestMethod]
        [DataRow("ACTIVE",true)]
        [DataRow("INACTIVE",false)]
        public void ToggleAccountStatusCommand_ExistingUser_TogglesCorrectly(string initialAccountStatus, bool FinalAccountStatus)
        {
            // Arrange
            var viewModel = new UserProfileViewModel(userProfileService, null, null);
            Assert.IsFalse(userProfileRepository.GetProfileById(1) == null);
            userProfileRepository.UpdateAccountStatus(1, initialAccountStatus);

            viewModel.ToggleAccountStatusCommand();

            var userWithId1 = userProfileService.GetProfile(1);
            Assert.AreEqual(FinalAccountStatus, userWithId1.ActiveAccount);

        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up any test data from the database if necessary
        }
    }
}
