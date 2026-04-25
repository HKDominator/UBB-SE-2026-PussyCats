using System;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying and managing the user's profile, including profile details,
    /// avatar, navigation, and related actions.
    /// </summary>
    public sealed partial class UserProfileView : Page
    {
        private static readonly int DefaultUserId = 1;
        private static readonly int DefaultExperienceProgressMaximum = 250;
        private static readonly int DefaultExperienceProgressValue = 150;

        private int currentUserId = DefaultUserId;
        public UserProfileViewModel UserProfileViewModel { get; private set; }
        private bool isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();
            ISkillTestRepository skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileRepository userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            UserProfileService userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);
            IImageStorageService imageStorageService = new ImageStorageService();
            ICompletenessService completenessService = new CompletenessService();
            UserProfileViewModel = new UserProfileViewModel(userProfileService, imageStorageService, completenessService);

            UserProfileViewModel.OnLevelUpdated += RenderLevelDisplay;
            this.DataContext = UserProfileViewModel;

            buttonEdit.Click += OnEditProfileClick;
            buttonOldTests.Click += OnGoToOldTestsClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            BindData();
        }

        private async void BindData()
        {
            isBinding = true;

            int dummyUserIdentity = DefaultUserId;
            await UserProfileViewModel.LoadUserAsync(dummyUserIdentity);

            if (!string.IsNullOrEmpty(UserProfileViewModel.ErrorMessage))
            {
                errorLabel.Text = UserProfileViewModel.ErrorMessage;
                isBinding = false;
                return;
            }

            if (UserProfileViewModel.UserProfile != null)
            {
                firstNameLabel.Text = $"First Name: {UserProfileViewModel.UserProfile.FirstName}";
                lastNameLabel.Text = $"Last Name: {UserProfileViewModel.UserProfile.LastName}";
                emailLabel.Text = $"Email: {UserProfileViewModel.UserProfile.Email}";
                phoneLabel.Text = $"Phone: {UserProfileViewModel.UserProfile.PhoneNumber}";
                githubAccountLabel.Text = $"GitHub: {UserProfileViewModel.UserProfile.GitHub}";
                linkedinAccountLabel.Text = $"LinkedIn: {UserProfileViewModel.UserProfile.LinkedIn}";

                string displayGender = UserProfileViewModel.UserProfile.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                {
                    displayGender = "Not specified";
                }

                Debug.WriteLine($"[BindData] ErrorMessage: '{UserProfileViewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfile is null: {UserProfileViewModel.UserProfile == null}");

                genderLabel.Text = $"Gender: {displayGender}";
                universityLabel.Text = $"University: {UserProfileViewModel.UserProfile.University}";
                countryLabel.Text = $"Country: {UserProfileViewModel.UserProfile.Country}";
                // addressLabel.Text = $"Address: {ViewModel.UserProfile.Address}";
                cityLabel.Text = $"City: {UserProfileViewModel.UserProfile.City}";
                graduationYearLabel.Text = $"Graduation Year: {UserProfileViewModel.UserProfile.ExpectedGraduationYear}";
                freshnessLabel.Text = UserProfileViewModel.FreshnessText;

                string testResultDisplay = "Not taken yet";
                if (!string.IsNullOrEmpty(UserProfileViewModel.UserProfile.PersonalityTestResult))
                {
                    // Try parsing the string to the JobRole enum
                    if (Enum.TryParse<JobRole>(UserProfileViewModel.UserProfile.PersonalityTestResult, out var jobRole))
                    {
                        var jobRoleToDisplayNameConverter = new Converters.JobRoleToDisplayNameConverter();

                        // Convert the enum value to the display string
                        testResultDisplay = jobRoleToDisplayNameConverter.Convert(jobRole, typeof(string), null, string.Empty).ToString();
                    }
                    else
                    {
                        // Fallback if the string couldn't be parsed
                        testResultDisplay = UserProfileViewModel.UserProfile.PersonalityTestResult;
                    }
                }
                personalityTestResultLabel.Text = $"Personality Test Result: {testResultDisplay}";

                LevelTitleText.Text = "Level 2 - Apprentice";
                ExperienceProgressBar.Maximum = DefaultExperienceProgressMaximum;
                ExperienceProgressBar.Value = DefaultExperienceProgressValue;
                ExperienceCountText.Text = $"{DefaultExperienceProgressValue} / {DefaultExperienceProgressMaximum} XP";

                checkAccountStatus.IsOn = UserProfileViewModel.UserProfile.ActiveAccount;

                if (!string.IsNullOrEmpty(UserProfileViewModel.UserProfile.ProfilePicture))
                {
                    publicAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(UserProfileViewModel.UserProfile.ProfilePicture));
                }
                else
                {
                    publicAvatar.ProfilePicture = null;
                }

                if (!string.IsNullOrEmpty(UserProfileViewModel.UserProfile.PersonalityTestResult))
                {
                    buttonPersonalityTest.Content = "Retake Personality Test";
                }
                else
                {
                    buttonPersonalityTest.Content = "Take Personality Test";
                }

                completenessBar.Update(UserProfileViewModel.CompletenessPercentage, UserProfileViewModel.NextEmptyFieldPrompt);

                UserProfileViewModel.RecalculateLevelCommand();
                RenderLevelDisplay();
            }
            else
            {
                return;
            }

            isBinding = false;
        }

        private void RenderLevelDisplay()
        {
            if (UserProfileViewModel.UserProfile == null)
            {
                return;
            }

            if (UserProfileViewModel.UserProfile.UserLevel == null)
            {
                return;
            }

            LevelTitleText.Text = $"Level {UserProfileViewModel.UserProfile.UserLevel.LevelNumber} — {UserProfileViewModel.UserProfile.UserLevel.Title}";

            ExperienceProgressBar.Value = UserLevelService.GetLevelProgressPercent(UserProfileViewModel.TotalExperiencePoints, UserProfileViewModel.UserProfile.UserLevel);

            int experienceTillNext = UserLevelService.GetXpToNextLevel(UserProfileViewModel.TotalExperiencePoints, UserProfileViewModel.UserProfile.UserLevel);
            ExperienceCountText.Text = experienceTillNext > 0
                ? $"{UserProfileViewModel.TotalExperiencePoints} XP — {experienceTillNext} XP needed for next level"
                : $"{UserProfileViewModel.TotalExperiencePoints} XP — Max level reached!";
        }

        private async void OnAvatarUploadClick(object sender, RoutedEventArgs routedEventArguments)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    UserProfileViewModel.UploadAvatarCommand(stream, file.Name);
                    BindData();
                }
            }
        }

        private void OnAvatarRemoveClick(object sender, RoutedEventArgs routedEventArguments)
        {
            UserProfileViewModel.RemoveAvatarCommand();
            BindData();
        }

        private void OnStatusToggle(object sender, RoutedEventArgs routedEventArguments)
        {
            if (isBinding)
            {
                return;
            }

            if (UserProfileViewModel?.UserProfile != null)
            {
                UserProfileViewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (UserProfileViewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ProfileFormView), UserProfileViewModel.UserProfile);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormView));
            }
        }

        private void OnPreviewCVClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (UserProfileViewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ExportCVView), UserProfileViewModel.UserProfile.UserId);
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (UserProfileViewModel.UserProfile == null)
            {
                return;
            }

            // UserProfileViewModel.UserProfile.UserId = currentUserId;
            this.Frame.Navigate(typeof(TestDashboardView), UserProfileViewModel.UserProfile);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (UserProfileViewModel.UserProfile == null)
            {
                return;
            }
            this.Frame.Navigate(typeof(PublicProfileView), UserProfileViewModel.UserProfile);
        }

        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs routedEventArguments)
        {
            Frame.Navigate(typeof(CompatibilityOverviewView), currentUserId);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs routedEventArguments)
        {
            UserProfileViewModel.TakePersonalityTestCommand();
        }

        private void OnViewDocumentsClick(object sender, RoutedEventArgs routedEventArguments)
        {
            Frame.Navigate(typeof(DocumentsListView));
        }

        private void OnMatchmakingHistoryClick(object sender, RoutedEventArgs routedEventArguments)
        {
            Frame.Navigate(typeof(MatchHistoryView));
        }
    }
}