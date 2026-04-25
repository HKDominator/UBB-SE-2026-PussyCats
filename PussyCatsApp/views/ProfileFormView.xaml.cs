using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying and editing the user profile form, including data binding, validation, and navigation.
    /// </summary>
    public sealed partial class ProfileFormView : Page
    {
        private ProfileFormViewModel profileFormViewModel;

        public ProfileFormView()
        {
            this.InitializeComponent();

            profileFormViewModel = ProfileFormViewModel.Create();

            PopulateGraduationYears();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            if (navigationEventArguments.Parameter is UserProfile profile)
            {
                profileFormViewModel.LoadProfile(profile);
                LoadViewFromViewModel();
            }
            else
            {
                profileFormViewModel.LoadProfile(new UserProfile());
            }
        }

        private void PopulateGraduationYears()
        {
            foreach (var year in profileFormViewModel.GraduationYears)
            {
                GraduationYearComboBox.Items.Add(year.ToString());
            }
        }

        private void LoadViewFromViewModel()
        {
            FirstNameTextBox.Text = profileFormViewModel.FirstName;
            LastNameTextBox.Text = profileFormViewModel.LastName;
            AgeNumberBox.Value = profileFormViewModel.Age;

            if (!string.IsNullOrEmpty(profileFormViewModel.Gender))
            {
                SelectGender(profileFormViewModel.Gender);
            }

            EmailTextBox.Text = profileFormViewModel.Email;
            GitHubTextBox.Text = profileFormViewModel.GitHub;
            LinkedInTextBox.Text = profileFormViewModel.LinkedIn;
            UniversityAutoSuggest.Text = profileFormViewModel.University;
            AddressTextBox.Text = profileFormViewModel.Address;
            MotivationTextBox.Text = profileFormViewModel.Motivation;

            if (!string.IsNullOrEmpty(profileFormViewModel.PhonePrefix))
            {
                SelectPhonePrefix(profileFormViewModel.PhonePrefix);
            }

            PhoneNumberTextBox.Text = profileFormViewModel.PhoneNumber;

            if (!string.IsNullOrEmpty(profileFormViewModel.Country))
            {
                SelectCountry(profileFormViewModel.Country);
            }

            // Set city AFTER country selection to avoid CountryComboBox_SelectionChanged clearing it
            CityTextBox.Text = profileFormViewModel.City;

            if (profileFormViewModel.ExpectedGraduationYear > 0)
            {
                SelectGraduationYear(profileFormViewModel.ExpectedGraduationYear);
            }

            SkillsItemsRepeater.ItemsSource = profileFormViewModel.Skills;
            WorkExperienceItemsRepeater.ItemsSource = profileFormViewModel.WorkExperiences;
            ProjectsItemsRepeater.ItemsSource = profileFormViewModel.Projects;
            ActivitiesItemsRepeater.ItemsSource = profileFormViewModel.ExtraCurricularActivities;
            DisabilitiesCheckBox.IsChecked = profileFormViewModel.HasDisabilities;
        }

        private void SyncViewToViewModel()
        {
            profileFormViewModel.FirstName = FirstNameTextBox.Text;
            profileFormViewModel.LastName = LastNameTextBox.Text;
            profileFormViewModel.Age = AgeNumberBox.Value;
            profileFormViewModel.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            profileFormViewModel.Email = EmailTextBox.Text;
            profileFormViewModel.PhonePrefix = (PhonePrefixComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            profileFormViewModel.PhoneNumber = PhoneNumberTextBox.Text;
            profileFormViewModel.GitHub = GitHubTextBox.Text;
            profileFormViewModel.LinkedIn = LinkedInTextBox.Text;
            profileFormViewModel.Country = (CountryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            profileFormViewModel.City = CityTextBox.Text;
            profileFormViewModel.University = UniversityAutoSuggest.Text;
            profileFormViewModel.Address = AddressTextBox.Text;
            profileFormViewModel.Motivation = MotivationTextBox.Text;
            profileFormViewModel.ExpectedGraduationYear = int.TryParse(GraduationYearComboBox.SelectedItem?.ToString(), out var yr) ? yr : 0;
            // profileFormViewModel.HasDisabilities = DisabilitiesCheckBox.IsChecked == true;
        }

        private async void UploadCVButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".xml");

            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, windowHandle);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string content = await FileIO.ReadTextAsync(file);
                profileFormViewModel.ProcessCVFile(content, file.FileType);
                LoadViewFromViewModel();
                ShowInformationBarFromViewModel();
            }
        }

        private void ShowInformationBarFromViewModel()
        {
            CVUploadInformationBar.Message = profileFormViewModel.InfoBarMessage;
            CVUploadInformationBar.Severity = (InfoBarSeverity)profileFormViewModel.InfoBarSeverity;
            CVUploadInformationBar.IsOpen = profileFormViewModel.IsInfoBarOpen;
            CVStatusText.Text = profileFormViewModel.CvStatusText;
        }

        private void ShowInformationBar(string message, InfoBarSeverity severity)
        {
            CVUploadInformationBar.Message = message;
            CVUploadInformationBar.Severity = severity;
            CVUploadInformationBar.IsOpen = true;
        }

        private void ShowFormValidation(string message, InfoBarSeverity severity)
        {
            FormValidationInformationBar.Message = message;
            FormValidationInformationBar.Severity = severity;
            FormValidationInformationBar.IsOpen = true;
        }

        private void SelectGender(string gender)
        {
            foreach (ComboBoxItem item in GenderComboBox.Items)
            {
                if (item.Content.ToString().Equals(gender, StringComparison.OrdinalIgnoreCase))
                {
                    GenderComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectCountry(string country)
        {
            foreach (ComboBoxItem item in CountryComboBox.Items)
            {
                if (item.Content.ToString().Equals(country, StringComparison.OrdinalIgnoreCase))
                {
                    CountryComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectPhonePrefix(string prefix)
        {
            foreach (ComboBoxItem item in PhonePrefixComboBox.Items)
            {
                if (item.Content.ToString() == prefix)
                {
                    PhonePrefixComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SelectGraduationYear(int year)
        {
            string yearStr = year.ToString();
            foreach (var item in GraduationYearComboBox.Items)
            {
                if (item.ToString() == yearStr)
                {
                    GraduationYearComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void UniversityAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs autoSuggestBoxTextChangedEventArguments)
        {
            if (autoSuggestBoxTextChangedEventArguments.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = profileFormViewModel.FilterUniversities(sender.Text);
            }
        }

        private void UniversityAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs autoSuggestBoxQuerySubmittedEventArguments)
        {
            if (autoSuggestBoxQuerySubmittedEventArguments.ChosenSuggestion != null)
            {
                sender.Text = autoSuggestBoxQuerySubmittedEventArguments.ChosenSuggestion.ToString();
            }
        }

        private void SkillsAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs autoSuggestBoxTextChangedEventArguments)
        {
            if (autoSuggestBoxTextChangedEventArguments.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = profileFormViewModel.FilterSkillSuggestions(sender.Text);
            }
        }

        private void SkillsAutoSuggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs autoSuggestBoxQuerySubmittedEventArguments)
        {
            string skillToAdd = autoSuggestBoxQuerySubmittedEventArguments.ChosenSuggestion?.ToString() ?? sender.Text;
            profileFormViewModel.AddSkill(skillToAdd);
            sender.Text = string.Empty;
            ShowInformationBarFromViewModel();
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            profileFormViewModel.AddSkill(SkillsAutoSuggest.Text);
            SkillsAutoSuggest.Text = string.Empty;
            ShowInformationBarFromViewModel();
        }

        private void RemoveSkillButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (sender is Button button && button.Tag is string skill)
            {
                profileFormViewModel.RemoveSkill(skill);
            }
        }

        private void AddWorkExperienceButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            profileFormViewModel.AddWorkExperience();
            ShowInformationBarFromViewModel();
        }

        private void RemoveWorkExperienceButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (sender is Button button && button.Tag is WorkExperience workExperience)
            {
                profileFormViewModel.RemoveWorkExperience(workExperience);
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            profileFormViewModel.AddProject();
            ShowInformationBarFromViewModel();
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is Button button && button.Tag is Project project)
            {
                profileFormViewModel.RemoveProject(project);
            }
        }

        private void AddActivityButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            profileFormViewModel.AddExtraCurricularActivity();
            ShowInformationBarFromViewModel();
        }

        private void RemoveActivityButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (sender is Button button && button.Tag is ExtraCurricularActivity activity)
            {
                profileFormViewModel.RemoveExtraCurricularActivity(activity);
            }
        }

        private void NameTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs textBoxBeforeTextChangingEventArguments)
        {
            textBoxBeforeTextChangingEventArguments.Cancel = textBoxBeforeTextChangingEventArguments.NewText.Any(character => char.IsDigit(character));
        }

        private void PhoneNumberTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs textBoxBeforeTextChangingEventArguments)
        {
            textBoxBeforeTextChangingEventArguments.Cancel = textBoxBeforeTextChangingEventArguments.NewText.Any(character => !char.IsDigit(character));
        }

        private void EndDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs datePickerValueChangedEventArguments)
        {
            try
            {
                if (sender is DatePicker endPicker && endPicker.Parent is Grid dateGrid)
                {
                    var startPicker = dateGrid.Children.OfType<DatePicker>().FirstOrDefault(datePicker => datePicker != endPicker);
                    if (startPicker != null && datePickerValueChangedEventArguments.NewDate.Date < startPicker.Date.Date)
                    {
                        ShowFormValidation("End date cannot be before start date. Please correct it before saving.", InfoBarSeverity.Warning);
                    }
                    else
                    {
                        FormValidationInformationBar.IsOpen = false;
                    }
                }
            }
            catch
            {
                // Prevent any crash from date picker events
            }
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArguments)
        {
            CityTextBox.Text = string.Empty;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            SyncViewToViewModel();

            bool success = profileFormViewModel.SaveProfile();
            ShowInformationBarFromViewModel();

            if (!success)
            {
                ShowFormValidation(profileFormViewModel.InfoBarMessage, (InfoBarSeverity)profileFormViewModel.InfoBarSeverity);
                return;
            }

            FormValidationInformationBar.IsOpen = false;

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(UserProfileView));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(UserProfileView));
            }
        }
        private void EditPreferencesButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            SyncViewToViewModel();

            Frame.Navigate(typeof(Views.PreferencesView));
        }
    }
}