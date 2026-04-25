using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Services;
using PussyCatsApp.Repositories;
using PussyCatsApp.Configuration;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for displaying the user's match history and related statistics, handling navigation and data loading.
    /// </summary>
    public sealed partial class MatchHistoryView : Page
    {
        private MatchHistoryViewModel matchHistoryViewModel;
        private static readonly int DefaultUserId = 1;

        public MatchHistoryView()
        {
            this.InitializeComponent();
            this.Loaded += MatchHistoryView_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            if (navigationEventArguments.Parameter is MatchHistoryViewModel otherMatchHistoryViewModel)
            {
                matchHistoryViewModel = otherMatchHistoryViewModel;
            }
        }

        private void MatchHistoryView_Loaded(object sender, RoutedEventArgs routedEventArguments)
        {
            if (matchHistoryViewModel == null)
            {
                IMatchService matchService = new MatchService(new MatchRepository(DatabaseConfiguration.GetConnectionString()));
                matchHistoryViewModel = new MatchHistoryViewModel(DefaultUserId, matchService);
            }

            LoadMatches();
            LoadStatistics();
        }

        private void LoadMatches()
        {
            matchHistoryViewModel.LoadMatches();

            string error = matchHistoryViewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                return;
            }

            MatchesListView.ItemsSource = matchHistoryViewModel.GetMatches();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void LoadStatistics()
        {
            matchHistoryViewModel.LoadStatistics();

            string error = matchHistoryViewModel.GetErrorMessage();
            if (!string.IsNullOrEmpty(error))
            {
                return;
            }

            var stats = matchHistoryViewModel.GetStatistics();

            if (stats != null)
            {
                totalMatchesLabel.Text = $"Total Matches: {stats.TotalMatches}";
                matchesLastMonthLabel.Text = $"Last Month: {stats.MatchesLastMonth}";
                matchesLastSixMonthsLabel.Text = $"Last 6 Months: {stats.MatchesLastSixMonths}";
                matchesLastYearLabel.Text = $"Last Year: {stats.MatchesLastYear}";

                if (stats.MatchesPerPosition != null)
                {
                    var positionData = stats.MatchesPerPosition
                        .Select(keyValuePair => new
                        {
                            JobRole = keyValuePair.Key,
                            Count = keyValuePair.Value
                        }).ToList();

                    PositionStatsListView.ItemsSource = positionData;
                }
            }
        }
    }
}
