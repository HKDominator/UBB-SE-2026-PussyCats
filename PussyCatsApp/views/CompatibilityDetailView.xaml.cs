using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Models;
using PussyCatsApp.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompatibilityDetailView : Page
    {
        private static readonly double InsufficientDataScore = -1;
        private static readonly int ScoreRoundingDecimals = 1;

        private CompatibilityDetailViewModel compatibilityDetailViewModel;

        public CompatibilityDetailView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArguments)
        {
            base.OnNavigatedTo(navigationArguments);
            RoleResult result = navigationArguments.Parameter as RoleResult;
            compatibilityDetailViewModel = new CompatibilityDetailViewModel();
            compatibilityDetailViewModel.LoadResult(result);
        }

        private void Page_Loaded(object sender, RoutedEventArgs loadArguments)
        {
            LoadDetail();
        }

        private void LoadDetail()
        {
            roleNameLabel.Text = compatibilityDetailViewModel.GetRoleName();

            double score = compatibilityDetailViewModel.GetMatchScore();
            if (score == InsufficientDataScore)
            {
                matchScoreLabel.Text = "Score: Insufficient Data";
            }
            else
            {
                matchScoreLabel.Text = "Match Score: " + Math.Round(score, ScoreRoundingDecimals) + "%";
            }

            ShowSuggestions();
        }

        private void ShowSuggestions()
        {
            List<Suggestion> suggestions = compatibilityDetailViewModel.GetSuggestions();

            if (suggestions == null || suggestions.Count == 0)
            {
                suggestionsList.Visibility = Visibility.Collapsed;
                noSuggestionsLabel.Visibility = Visibility.Visible;
                return;
            }

            List<object> displayItems = new List<object>();
            foreach (Suggestion suggestion in suggestions)
            {
                displayItems.Add(new
                {
                    suggestion.SkillName,
                    suggestion.GroupName,
                    GainDisplay = "Potential gain: +" + Math.Round(suggestion.GainScore, ScoreRoundingDecimals) + "%"
                });
            }

            suggestionsList.ItemsSource = displayItems;
            suggestionsList.Visibility = Visibility.Visible;
            noSuggestionsLabel.Visibility = Visibility.Collapsed;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        public async Task ShowError(string message)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Error";
            dialog.Content = message;
            dialog.CloseButtonText = "OK";
            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }

        public async Task ShowMessage(string message)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Information";
            dialog.Content = message;
            dialog.CloseButtonText = "OK";
            dialog.XamlRoot = this.XamlRoot;
            await dialog.ShowAsync();
        }
    }
}
