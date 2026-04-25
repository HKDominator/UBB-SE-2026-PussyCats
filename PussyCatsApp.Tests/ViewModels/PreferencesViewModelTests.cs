using Moq;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class PreferencesViewModelTests
    {

        private Mock<IPreferenceService> mockPreferenceService;
        private PreferencesViewModel viewModel;
        private const int testUserId = 1;

        [TestInitialize]
        public void Setup()
        {
            mockPreferenceService = new Mock<IPreferenceService>();
            viewModel = new PreferencesViewModel(mockPreferenceService.Object, testUserId);
        }

        [TestMethod]
        public void LoadPreferences_LoadsJobRoles_FromService()
        {
            Preference preference = new Preference
            {
                PreferenceId = 1,
                UserId = testUserId,
                PreferenceType = "JobRole",
                Value = "BackendDeveloper"
            };
            mockPreferenceService.Setup(s => s.GetByUserId(testUserId)).Returns(new List<Preference>
            {
                preference
            });
            viewModel.LoadPreferences();
            Assert.AreEqual(1, viewModel.GetSelectedJobRoles().Count);
            Assert.IsTrue(viewModel.GetSelectedJobRoles().Contains(JobRole.BackendDeveloper));
        }

        [TestMethod]
        public void LoadPreferences_LoadsWorkMode_FromService()
        {
            Preference preference = new Preference
            {
                PreferenceId = 1,
                UserId = testUserId,
                PreferenceType = "WorkMode",
                Value = "Remote"
            };
            mockPreferenceService.Setup(s => s.GetByUserId(testUserId)).Returns(new List<Preference>
            {
                preference
            });
            viewModel.LoadPreferences();
            Assert.AreEqual(WorkMode.Remote, viewModel.GetSelectedWorkMode());
        }

        [TestMethod]
        public void LoadPreferences_LoadsLocation_FromService()
        {
            string location = "Cluj-Napoca";
            Preference preference = new Preference
            {
                PreferenceId = 1,
                UserId = testUserId,
                PreferenceType = "Location",
                Value = location
            };
            mockPreferenceService.Setup(s => s.GetByUserId(testUserId)).Returns(new List<Preference>
            {
                preference
            });
            viewModel.LoadPreferences();
            Assert.AreEqual(location, viewModel.GetPreferredLocation());
        }

        [TestMethod]
        public void ToggleJobRole_AddsRole_WhenNotSelected()
        {
            viewModel.ToggleJobRole(JobRole.DataAnalyst);
            Assert.IsTrue(viewModel.GetSelectedJobRoles().Contains(JobRole.DataAnalyst));
        }

        [TestMethod]
        public void ToggleJobRole_RemovesRole_WhenAlreadySelected()
        {
            viewModel.ToggleJobRole(JobRole.DataAnalyst);
            viewModel.ToggleJobRole(JobRole.DataAnalyst);
            Assert.IsFalse(viewModel.GetSelectedJobRoles().Contains(JobRole.DataAnalyst));
        }

        [TestMethod]
        public void ToggleJobRole_FailsToAdd_WhenMaximumRolesSelected()
        {
            viewModel.ToggleJobRole(JobRole.BackendDeveloper);
            viewModel.ToggleJobRole(JobRole.FrontendDeveloper);
            viewModel.ToggleJobRole(JobRole.DataAnalyst);
            viewModel.ToggleJobRole(JobRole.AIMLEngineer);
            Assert.IsFalse(viewModel.GetSelectedJobRoles().Contains(JobRole.AIMLEngineer));
        }


        [TestMethod]
        public void ToggleJobRole_UpdatesErrorMessage_WhenMaximumRolesExceeded()
        {
            int MaximumJobRolesAllowed = 3;

            viewModel.ToggleJobRole(JobRole.BackendDeveloper);
            viewModel.ToggleJobRole(JobRole.FrontendDeveloper);
            viewModel.ToggleJobRole(JobRole.DataAnalyst);
            viewModel.ToggleJobRole(JobRole.AIMLEngineer);
            Assert.AreEqual($"You can select a maximum of {MaximumJobRolesAllowed} job roles.", viewModel.GetErrorMessage());
        }
    }
}
