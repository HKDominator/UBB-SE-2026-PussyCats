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
        private int currentUserId = 1;
        public UserProfileViewModel ViewModel { get; private set; }
        private bool isBinding = false;

        public UserProfileView()
        {
            this.InitializeComponent();
            ISkillTestRepository skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileRepository userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            UserProfileService userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);
            IImageStorageService imageStorageService = new ImageStorageService();
            ICompletenessService completenessService = new CompletenessService();
            ViewModel = new UserProfileViewModel(userProfileService, imageStorageService, completenessService);

            ViewModel.OnLevelUpdated += RenderLevelDisplay;
            this.DataContext = ViewModel;

            btnEdit.Click += OnEditProfileClick;
            btnOldTests.Click += OnGoToOldTestsClick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BindData();
        }

        private async void BindData()
        {
            isBinding = true;

            int dummyUserId = 1;
            await ViewModel.LoadUserAsync(dummyUserId);

            if (!string.IsNullOrEmpty(ViewModel.ErrorMessage))
            {
                lblError.Text = ViewModel.ErrorMessage;
                isBinding = false;
                return;
            }

            if (ViewModel.UserProfile != null)
            {
                lblFirstName.Text = $"First Name: {ViewModel.UserProfile.FirstName}";
                lblLastName.Text = $"Last Name: {ViewModel.UserProfile.LastName}";
                lblEmail.Text = $"Email: {ViewModel.UserProfile.Email}";
                lblPhone.Text = $"Phone: {ViewModel.UserProfile.PhoneNumber}";
                lblGithubAccount.Text = $"GitHub: {ViewModel.UserProfile.GitHub}";
                lblLinkedinAccount.Text = $"LinkedIn: {ViewModel.UserProfile.LinkedIn}";

                string displayGender = ViewModel.UserProfile.Gender;
                if (string.IsNullOrEmpty(displayGender) || (displayGender != "Male" && displayGender != "Female"))
                {
                    displayGender = "Not specified";
                }

                Debug.WriteLine($"[BindData] ErrorMessage: '{ViewModel.ErrorMessage}'");
                Debug.WriteLine($"[BindData] userProfile is null: {ViewModel.UserProfile == null}");

                lblGender.Text = $"Gender: {displayGender}";
                lblUniversity.Text = $"University: {ViewModel.UserProfile.University}";
                lblCountry.Text = $"Country: {ViewModel.UserProfile.Country}";
                // lblAddress.Text = $"Address: {ViewModel.UserProfile.Address}";
                lblCity.Text = $"City: {ViewModel.UserProfile.City}";
                lblGraduationYear.Text = $"Graduation Year: {ViewModel.UserProfile.ExpectedGraduationYear}";
                lblFreshness.Text = ViewModel.FreshnessText;

                string testResultDisplay = "Not taken yet";
                if (!string.IsNullOrEmpty(ViewModel.UserProfile.PersonalityTestResult))
                {
                    // Try parsing the string to the JobRole enum
                    if (Enum.TryParse<JobRole>(ViewModel.UserProfile.PersonalityTestResult, out var jobRole))
                    {
                        var converter = new Converters.JobRoleToDisplayNameConverter();

                        // Convert the enum value to the display string
                        testResultDisplay = converter.Convert(jobRole, typeof(string), null, string.Empty).ToString();
                    }
                    else
                    {
                        // Fallback if the string couldn't be parsed
                        testResultDisplay = ViewModel.UserProfile.PersonalityTestResult;
                    }
                }
                lblPersonalityTestResult.Text = $"Personality Test Result: {testResultDisplay}";

                LevelTitleText.Text = "Level 2 - Apprentice";
                XpProgressBar.Maximum = 250;
                XpProgressBar.Value = 150;
                XpCountText.Text = "150 / 250 XP";

                chkAccountStatus.IsOn = ViewModel.UserProfile.ActiveAccount;

                if (!string.IsNullOrEmpty(ViewModel.UserProfile.ProfilePicture))
                {
                    pbAvatar.ProfilePicture =
                        new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                            new Uri(ViewModel.UserProfile.ProfilePicture));
                }
                else
                {
                    pbAvatar.ProfilePicture = null;
                }

                if (!string.IsNullOrEmpty(ViewModel.UserProfile.PersonalityTestResult))
                {
                    btnPersonalityTest.Content = "Retake Personality Test";
                }
                else
                {
                    btnPersonalityTest.Content = "Take Personality Test";
                }

                completenessBar.Update(ViewModel.CompletenessPercentage, ViewModel.NextEmptyFieldPrompt);

                ViewModel.RecalculateLevelCommand();
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
            if (ViewModel.UserProfile == null)
            {
                return;
            }

            if (ViewModel.UserProfile.UserLevel == null)
            {
                return;
            }

            LevelTitleText.Text = $"Level {ViewModel.UserProfile.UserLevel.LevelNumber} — {ViewModel.UserProfile.UserLevel.Title}";

            XpProgressBar.Value = UserLevelService.GetLevelProgressPercent(ViewModel.TotalExperiencePoints, ViewModel.UserProfile.UserLevel);

            int xpToNext = UserLevelService.GetExperiencePointsToNextLevel(ViewModel.TotalExperiencePoints, ViewModel.UserProfile.UserLevel);
            XpCountText.Text = xpToNext > 0
                ? $"{ViewModel.TotalExperiencePoints} XP — {xpToNext} XP needed for next level"
                : $"{ViewModel.TotalExperiencePoints} XP — Max level reached!";
        }

        private async void OnAvatarUploadClick(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

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
                    ViewModel.UploadAvatarCommand(stream, file.Name);
                    BindData();
                }
            }
        }

        private void OnAvatarRemoveClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveAvatarCommand();
            BindData();
        }

        private void OnStatusToggle(object sender, RoutedEventArgs e)
        {
            if (isBinding)
            {
                return;
            }

            if (ViewModel?.UserProfile != null)
            {
                ViewModel.ToggleAccountStatusCommand();
                BindData();
            }
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ProfileFormView), ViewModel.UserProfile);
            }
            else
            {
                Frame.Navigate(typeof(ProfileFormView));
            }
        }

        private void OnPreviewCVClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfile != null)
            {
                Frame.Navigate(typeof(ExportCVView), ViewModel.UserProfile.UserId);
            }
        }

        private void OnGoToOldTestsClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfile == null)
            {
                return;
            }

            // ViewModel.UserProfile.UserId = currentUserId;
            this.Frame.Navigate(typeof(TestDashboardView), ViewModel.UserProfile);
        }

        private void OnSeePublicProfileClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.UserProfile == null)
            {
                return;
            }
            this.Frame.Navigate(typeof(PublicProfileView), ViewModel.UserProfile);
        }

        private void OnCompatibilityAnalyzerClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompatibilityOverviewView), currentUserId);
        }

        private void OnPersonalityTestClick(object sender, RoutedEventArgs e)
        {
            ViewModel.TakePersonalityTestCommand();
        }

        private void OnViewDocumentsClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DocumentsListView));
        }

        private void OnMatchmakingHistoryClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MatchHistoryView));
        }
    }
}