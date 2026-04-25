using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.Views;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for displaying and managing a user's profile,
    /// including profile data, avatar, account status, and related operations.
    /// </summary>
    public partial class UserProfileViewModel : ObservableObject
    {
        private IUserProfileService profileService;
        private IImageStorageService imageStorageService;
        private ICompletenessService completenessService;

        // Nested Export ViewModel
        public ExportCVViewModel ExportCvViewModel { get; }

        private UserProfile? userProfile;
        public UserProfile? UserProfile
        {
            get => userProfile;
            set => SetProperty(ref userProfile, value);
        }

        public bool IsLoading { get; set; }
        public int CompletenessPercentage { get; set; }
        public string NextEmptyFieldPrompt { get; set; } = string.Empty;
        public List<string> MissingFieldWarnings { get; set; } = new List<string>();
        public string ErrorMessage { get; set; } = string.Empty;
        public string FreshnessText { get; set; } = string.Empty;
        public int TotalExperiencePoints { get; private set; } = 0;

        public UserProfileViewModel(IUserProfileService userProfileService, IImageStorageService imageStorageService, ICompletenessService completenessService)
        {
            this.profileService = userProfileService;
            this.imageStorageService = imageStorageService;
            this.completenessService = completenessService;
        }

        public event Action OnLevelUpdated;
        public void RecalculateLevelCommand()
        {
            if (UserProfile == null)
            {
                return;
            }

            try
            {
                TotalExperiencePoints = profileService.RecalculateLevel(UserProfile);
                OnLevelUpdated?.Invoke();
            }
            catch (Exception exception)
            {
                ErrorMessage = $"Error recalculating user level: {exception.Message}";
            }
        }
        public async Task LoadUserAsync(int userId)
        {
            ErrorMessage = string.Empty;
            IsLoading = true;
            try
            {
                UserProfile = await Task.Run(() => profileService.GetProfile(userId));

                if (UserProfile != null)
                {
                    FreshnessText = Utilities.TimeFormatter.CalculateFreshnessLabel(UserProfile.LastUpdated);

                    CompletenessPercentage = completenessService.CalculateCompleteness(UserProfile);
                    NextEmptyFieldPrompt = completenessService.GetNextEmptyFieldPrompt(UserProfile);
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = $"Error loading user profile: {exception.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ToggleAccountStatusCommand()
        {
            if (UserProfile == null)
            {
                return;
            }
            string currentStatusString = UserProfile.ActiveAccount ? "ACTIVE" : "INACTIVE";
            profileService.ToggleAccountStatus(UserProfile.UserId, currentStatusString);
            UserProfile.ActiveAccount = !UserProfile.ActiveAccount;
        }

        public void UploadAvatarCommand(Stream fileStream, string fileName)
        {
            if (UserProfile == null)
            {
                return;
            }
            try
            {
                string newPath = imageStorageService.SaveImage(fileStream, fileName);
                profileService.UpdateProfilePicturePath(UserProfile.UserId, newPath);
                UserProfile.ProfilePicture = newPath;
            }
            catch (Exception exception)
            {
                ErrorMessage = $"Error uploading avatar: {exception.Message}";
                return;
            }
        }

        public void RemoveAvatarCommand()
        {
            if (!string.IsNullOrEmpty(UserProfile?.ProfilePicture))
            {
                imageStorageService.DeleteImage(UserProfile.ProfilePicture);
                profileService.RemoveProfilePicturePath(UserProfile.UserId);
                UserProfile.ProfilePicture = null;
            }
        }

        public string GetPersonalityButtonText()
        {
            if (UserProfile != null && string.IsNullOrEmpty(UserProfile.PersonalityTestResult))
            {
                return "TAKE PERSONALITY TEST";
            }
            return "RETAKE PERSONALITY TEST";
        }
        public void TakePersonalityTestCommand()
        {
            if (App.MainAppWindow is MainWindow mainWindow)
            {
                mainWindow.NavigationFrame.Navigate(typeof(PersonalityTestView), 1);
            }
        }
    }
}