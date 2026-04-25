using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying another user's public profile information.
    /// </summary>
    public sealed partial class ViewProfilePage : Page
    {
        private UserProfile userProfile;

        public ViewProfilePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            if (navigationEventArguments.Parameter is UserProfile profile)
            {
                userProfile = profile;
            }
            else
            {
                userProfile = new UserProfile();
            }
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            FirstNameText.Text = string.IsNullOrEmpty(userProfile.FirstName) ? "\u2014" : userProfile.FirstName;
            LastNameText.Text = string.IsNullOrEmpty(userProfile.LastName) ? "\u2014" : userProfile.LastName;
            AgeText.Text = userProfile.Age > 0 ? userProfile.Age.ToString() : "\u2014";
            GenderText.Text = string.IsNullOrEmpty(userProfile.Gender) ? "\u2014" : userProfile.Gender;
            EmailText.Text = string.IsNullOrEmpty(userProfile.Email) ? "\u2014" : userProfile.Email;
            PhoneText.Text = string.IsNullOrEmpty(userProfile.PhoneNumber) ? "\u2014" : userProfile.PhoneNumber;
            CountryText.Text = string.IsNullOrEmpty(userProfile.Country) ? "\u2014" : userProfile.Country;
            UniversityText.Text = string.IsNullOrEmpty(userProfile.University) ? "\u2014" : userProfile.University;
            GraduationYearText.Text = userProfile.ExpectedGraduationYear > 0 ? userProfile.ExpectedGraduationYear.ToString() : "\u2014";
            AddressText.Text = string.IsNullOrEmpty(userProfile.Address) ? "\u2014" : userProfile.Address;
            GitHubText.Text = string.IsNullOrEmpty(userProfile.GitHub) ? "\u2014" : userProfile.GitHub;
            LinkedInText.Text = string.IsNullOrEmpty(userProfile.LinkedIn) ? "\u2014" : userProfile.LinkedIn;
            MotivationText.Text = string.IsNullOrEmpty(userProfile.Motivation) ? "No motivation provided." : userProfile.Motivation;

            if (userProfile.Skills != null && userProfile.Skills.Count > 0)
            {
                SkillsRepeater.ItemsSource = userProfile.Skills;
                NoSkillsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoSkillsText.Visibility = Visibility.Visible;
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            Frame.Navigate(typeof(ProfileFormView), userProfile);
        }
    }
}