using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// Page that allows users to set and save their job preferences, including preferred roles
    /// (up to three), work mode, and geographic location for matchmaking purposes.
    /// </summary>
    public sealed partial class PreferencesView : Page
    {
        private PreferencesViewModel viewModel;

        private readonly Dictionary<JobRole, string> roleDisplayNames = new ()
        {
            { JobRole.FrontendDeveloper,      "Frontend Developer" },
            { JobRole.BackendDeveloper,        "Backend Developer" },
            { JobRole.UIUXDesigner,            "UI/UX Designer" },
            { JobRole.DevOpsEngineer,          "DevOps Engineer" },
            { JobRole.ProjectManager,          "Project Manager" },
            { JobRole.DataAnalyst,             "Data Analyst" },
            { JobRole.CybersecuritySpecialist, "Cybersecurity Specialist" },
            { JobRole.AIMLEngineer,            "AI/ML Engineer" }
        };

        public PreferencesView()
        {
            this.InitializeComponent();
            RolesListView.ItemsSource = roleDisplayNames.Values.ToList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int userId = e.Parameter is int id ? id : 1;
            viewModel = new PreferencesViewModel(new PreferenceService(new PreferenceRepository(DatabaseConfiguration.GetConnectionString())), userId);
            viewModel.LoadPreferences();
            PopulateUIFromViewModel();
        }

        /// <summary>
        /// Reads the loaded preferences from the ViewModel and reflects them in the UI controls.
        /// </summary>
        private void PopulateUIFromViewModel()
        {
            // --- Roles ---
            RolesListView.SelectionChanged -= RolesListView_SelectionChanged;

            RolesListView.SelectedItems.Clear();

            foreach (var role in viewModel.GetSelectedJobRoles())
            {
                if (roleDisplayNames.TryGetValue(role, out var displayName))
                {
                    var index = RolesListView.Items
                        .Cast<string>()
                        .ToList()
                        .IndexOf(displayName);

                    if (index >= 0)
                    {
                        RolesListView.SelectedItems.Add(RolesListView.Items[index]);
                    }
                }
            }

            RolesListView.SelectionChanged += RolesListView_SelectionChanged;

            // --- Work mode ---
            var savedWorkMode = viewModel.GetSelectedWorkMode();
            var workModeDisplay = savedWorkMode.ToString();

            foreach (var item in WorkModeComboBox.Items)
            {
                if (item is ComboBoxItem comboBoxItem &&
                    comboBoxItem.Tag?.ToString() == workModeDisplay)
                {
                    WorkModeComboBox.SelectedItem = comboBoxItem;
                    break;
                }
            }

            // --- Location ---
            LocationAutoSuggestBox.Text = viewModel.GetPreferredLocation();
        }

        private void RolesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems.Cast<string>())
            {
                var role = roleDisplayNames.First(kv => kv.Value == item).Key;
                viewModel.ToggleJobRole(role);
            }

            foreach (var item in e.RemovedItems.Cast<string>())
            {
                var role = roleDisplayNames.First(kv => kv.Value == item).Key;
                viewModel.ToggleJobRole(role);
            }

            string error = viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                RoleWarningText.Visibility = Visibility.Visible;

                RolesListView.SelectionChanged -= RolesListView_SelectionChanged;
                foreach (var item in e.AddedItems.Cast<string>())
                {
                    var role = roleDisplayNames.First(kv => kv.Value == item).Key;
                    if (!viewModel.GetSelectedJobRoles().Contains(role))
                    {
                        RolesListView.SelectedItems.Remove(item);
                    }
                }
                RolesListView.SelectionChanged += RolesListView_SelectionChanged;
            }
            else
            {
                RoleWarningText.Visibility = Visibility.Collapsed;
            }
        }

        private void LocationAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                viewModel.SearchLocation(sender.Text);
                sender.ItemsSource = viewModel.GetLocationSuggestions();
            }
        }

        private void LocationAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string chosen = args.SelectedItem.ToString();
            sender.Text = chosen;
            viewModel.SetLocation(chosen);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var workMode = (WorkModeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            if (!string.IsNullOrEmpty(workMode) && Enum.TryParse<WorkMode>(workMode, out var parsedMode))
            {
                viewModel.SetWorkMode(parsedMode);
            }

            viewModel.SetLocation(LocationAutoSuggestBox.Text);

            if (viewModel.GetSelectedJobRoles().Count == 0 ||
                WorkModeComboBox.SelectedItem == null ||
                string.IsNullOrEmpty(LocationAutoSuggestBox.Text))
            {
                ShowMessage("Please fill in all the fields before saving.", isError: true);
                return;
            }

            viewModel.SavePreferences();

            string error = viewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                ShowMessage(error, isError: true);
            }
            else
            {
                ShowMessage("Preferences successfully saved!", isError: false);
            }
        }

        private void ShowMessage(string text, bool isError)
        {
            SuccessMessage.Text = text;
            SuccessMessage.Foreground = new SolidColorBrush(
                isError ? Microsoft.UI.Colors.Red : Microsoft.UI.Colors.Green);
            SuccessMessage.Visibility = Visibility.Visible;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}