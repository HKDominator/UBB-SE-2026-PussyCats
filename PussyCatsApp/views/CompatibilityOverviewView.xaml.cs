using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// View for displaying an overview of compatibility results for different job roles,
    /// including navigation and error handling.
    /// </summary>
    public sealed partial class CompatibilityOverviewView : Page
    {
        private static readonly double InsufficientDataScore = -1;
        private static readonly int ScoreRoundingDecimals = 1;

        private CompatibilityOverviewViewModel compatibilityOverviewViewModel;

        public CompatibilityOverviewView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            int userIdentity = (int)navigationEventArguments.Parameter;

            IUserSkillRepository userSkillRepository = new UserSkillRepository(DatabaseConfiguration.GetConnectionString());
            ISkillGroupRepository skillGroupRepository = new SkillGroupRepository();
            ICompatibilityService compatibilityService = new CompatibilityService(userSkillRepository, skillGroupRepository);
            compatibilityOverviewViewModel = new CompatibilityOverviewViewModel(userIdentity, compatibilityService);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs routedEventArguments)
        {
            if (compatibilityOverviewViewModel == null)
            {
                await ShowError("CompatibilityOverviewViewModel was not provided.");
                return;
            }

            compatibilityOverviewViewModel.LoadAllRoles();

            if (!string.IsNullOrEmpty(compatibilityOverviewViewModel.GetErrorMessage()))
            {
                await ShowError(compatibilityOverviewViewModel.GetErrorMessage());
                return;
            }

            List<RoleResult> roleResults = compatibilityOverviewViewModel.GetRoleResults();
            List<object> displayItems = new List<object>();

            foreach (RoleResult result in roleResults)
            {
                string formattedName = string.Empty;
                if (result.JobRole == JobRole.UIUXDesigner)
                {
                    formattedName = "UI/UX Designer";
                }
                else if (result.JobRole == JobRole.AIMLEngineer)
                {
                    formattedName = "AI/ML Engineer";
                }
                else
                {
                    formattedName = FormatRoleName(result.JobRole.ToString());
                }

                displayItems.Add(new
                {
                    Result = result,
                    DisplayName = FormatRoleName(formattedName),
                    DisplayScore = result.MatchScore == InsufficientDataScore ? 0 : result.MatchScore,
                    DisplayPercentage = result.MatchScore == InsufficientDataScore
                        ? "Insufficient Data"
                        : Math.Round(result.MatchScore, ScoreRoundingDecimals) + "%"
                });
            }

            rolesList.ItemsSource = displayItems;
        }

        private string FormatRoleName(string raw)
        {
            var newString = new System.Text.StringBuilder();

            foreach (char character in raw)
            {
                if (char.IsUpper(character) && newString.Length > 0)
                {
                    newString.Append(' ');
                }

                newString.Append(character);
            }

            return newString.ToString();
        }

        private void RolesList_SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArguments)
        {
            if (rolesList.SelectedItem == null || compatibilityOverviewViewModel == null)
            {
                return;
            }

            dynamic selected = rolesList.SelectedItem;
            RoleResult result = selected.Result;

            compatibilityOverviewViewModel.OnRoleSelected(result.JobRole);
            NavigateToDetail(compatibilityOverviewViewModel.GetSelectedResult());
        }

        private void NavigateToDetail(RoleResult result)
        {
            if (result == null)
            {
                return;
            }

            Frame.Navigate(typeof(CompatibilityDetailView), result);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public async Task ShowError(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public async Task ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Information",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public async void ShowInsufficientData()
        {
            await ShowMessage("Role data is unavailable and the score cannot be calculated for this role.");
        }
    }
}