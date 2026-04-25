using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories;
using PussyCatsApp.Configuration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PublicProfileView : Page
    {
        private readonly PublicProfileViewModel publicProfileViewModel;
        public PublicProfileView()
        {
            this.InitializeComponent();

            var skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            var userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            var userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);
            publicProfileViewModel = new PublicProfileViewModel(userProfileService);
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            if (navigationEventArguments.Parameter is UserProfile profile)
            {
                publicProfileViewModel.LoadPublicProfileCommand(profile.UserId);
                if (publicProfileViewModel.IsAvailable)
                {
                    ProfileContentPanel.Visibility = Visibility.Visible;
                    ProfileUnavailableTextBlock.Visibility = Visibility.Collapsed;
                    UpdateUI(profile);
                }
                else
                {
                    ProfileContentPanel.Visibility = Visibility.Collapsed;
                    ProfileUnavailableTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private Uri GetValidUri(string url, string fallback)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return new Uri(fallback);
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            {
                return result;
            }

            return new Uri(fallback);
        }
        private void UpdateUI(UserProfile profile)
        {
            FirstNameLabel.Text = profile.FirstName;
            LastNameLabel.Text = profile.LastName;
            LevelLabel.Text = $"Level {profile.UserLevel.Title}";

            EmailLabel.Text = profile.Email;
            PhoneLabel.Text = profile.PhoneNumber;
            GenderLabel.Text = $"{profile.Gender}";

            UniversityLabel.Text = $"{profile.University}";
            GradYearLabel.Text = $"{profile.ExpectedGraduationYear}";
            CountryLabel.Text = $"{profile.Country}";
            AddressLabel.Text = $"{profile.Address}";

            GithubLink.NavigateUri = GetValidUri(profile.GitHub, "https://github.com");
            LinkedinLink.NavigateUri = GetValidUri(profile.LinkedIn, "https://linkedin.com");

            if (!string.IsNullOrEmpty(publicProfileViewModel.Profile.ProfilePicture))
            {
                ProfilePhoto.Source =
                    new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri(publicProfileViewModel.Profile.ProfilePicture));
            }
            else
            {
                ProfilePhoto.Source = null;
            }

            SkillTestsContainer.Children.Clear();
            foreach (var test in publicProfileViewModel.Tests)
            {
                var row = new TextBlock
                {
                    Text = $"• {test.Name}: {test.Score}%",
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    Margin = new Thickness(0, 5, 0, 5)
                };
                SkillTestsContainer.Children.Add(row);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
