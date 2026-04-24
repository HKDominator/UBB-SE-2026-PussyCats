using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestDashboardView : Page
    {
        private TestDashboardViewModel testDashboardViewModel;

        public TestDashboardView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is UserProfile profile && profile.UserId != 0)
            {
                var skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
                ISkillTestService skillTestService = new SkillTestService(skillTestRepository);

                var userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
                IUserProfileService userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);
                IImageStorageService imageStorageService = new ImageStorageService();
                ICompletenessService completenessService = new CompletenessService();
                var userProfileViewModel = new UserProfileViewModel(userProfileService, imageStorageService, completenessService);

                testDashboardViewModel = new TestDashboardViewModel(skillTestService, userProfileViewModel);
                testDashboardViewModel.LoadTests(profile);
                RenderTestCards();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Warning: Navigated to Dashboard without a valid UserID.");
            }
        }
        private void RenderTestCards()
        {
            TestCardsContainer.Children.Clear();
            foreach (SkillTestCardViewModel cardViewModel in testDashboardViewModel.TestCards)
            {
                SkillTestCardView cardView = new SkillTestCardView(cardViewModel);
                TestCardsContainer.Children.Add(cardView);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserProfileView));
        }

        private void GoToAllTestsButton_Click(object sender, RoutedEventArgs e)
        {
            testDashboardViewModel.GoToAllTestsCommand();
        }
    }
}
