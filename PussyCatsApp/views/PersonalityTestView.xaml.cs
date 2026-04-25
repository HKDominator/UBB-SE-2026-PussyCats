using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories.PersonalityTestRepo;
using PussyCatsApp.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// Personality Test page where users answer 24 questions to receive role recommendations.
    /// </summary>
    public sealed partial class PersonalityTestView : Page
    {
        private PersonalityTestViewModel personalityTestViewModel;
        private static readonly int DefaultUserId = 1;

        public PersonalityTestView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"PersonalityTestView initialization error: {exception.Message}");
                Debug.WriteLine($"Stack trace: {exception.StackTrace}");
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs routedEventArguments)
        {
            try
            {
                // Prefer using this page's Frame if it can go back
                if (Frame != null && Frame.CanGoBack)
                {
                    Frame.GoBack();
                    return;
                }

                // Otherwise, try the application's main window frame
                if (App.MainAppWindow is MainWindow mainWindow && mainWindow.NavigationFrame != null)
                {
                    var navigatedFrame = mainWindow.NavigationFrame;
                    if (navigatedFrame.CanGoBack)
                    {
                        navigatedFrame.GoBack();
                    }
                    else
                    {
                        navigatedFrame.Navigate(typeof(UserProfileView));
                    }
                    return;
                }

                // Fallback: navigate in this page's frame to the profile view
                Frame?.Navigate(typeof(UserProfileView));
            }
            catch (Exception exception)
            {
                Debug.WriteLine("[PersonalityTestView] Back navigation error: " + exception);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            // Get userId from navigation parameter
            int userIdentity = DefaultUserId; // Default value

            if (navigationEventArguments.Parameter is int passedUserIdentity)
            {
                userIdentity = passedUserIdentity;
            }

            InitializeViewModel(userIdentity);
        }
        private void InitializeViewModel(int userIdentity)
        {
            IPersonalityTestService personalityTestService = new PersonalityTestService(new PersonalityTestRepository(DatabaseConfiguration.GetConnectionString()));
            this.personalityTestViewModel = new PersonalityTestViewModel(userIdentity, personalityTestService);
            this.DataContext = this.personalityTestViewModel;
        }
    }
}
