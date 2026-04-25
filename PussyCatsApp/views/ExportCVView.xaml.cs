using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.Configuration;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View for exporting and previewing a user's CV as a PDF,
    /// handling navigation, loading, and integration with the export ViewModel.
    /// </summary>
    public sealed partial class ExportCVView : Page
    {
        public ExportCVViewModel ExportCVViewModel { get; private set; }
        private int userIdentity;

        public ExportCVView()
        {
            this.InitializeComponent();
            this.Loaded += OnPageLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            if (navigationEventArguments.Parameter is int passedUserId)
            {
                userIdentity = passedUserId;
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void OnPageLoaded(object sender, RoutedEventArgs routedEventArguments)
        {
            this.Loaded -= OnPageLoaded;

            await CvWebView.EnsureCoreWebView2Async();
            CvWebView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

            var templateFolder = Path.Combine(AppContext.BaseDirectory, "resources");
            CvWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "assets.local",
                templateFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            IPdfExportService pdfExportService = new PdfExportService(CvWebView);
            ISkillTestRepository skillTestRepository = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileRepository userProfileRepository = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            IUserProfileService userProfileService = new UserProfileService(skillTestRepository, userProfileRepository);

            ExportCVViewModel = new ExportCVViewModel(pdfExportService, userProfileService);
            ExportCVViewModel.UserId = userIdentity;

            this.DataContext = ExportCVViewModel;

            await ExportCVViewModel.LoadAndRenderCVAsync();
        }

        private async void CoreWebView2_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs coreWebView2NavigationStartingEventArguments)
        {
            if (coreWebView2NavigationStartingEventArguments.Uri.StartsWith("http") && !coreWebView2NavigationStartingEventArguments.Uri.Contains("assets.local"))
            {
                coreWebView2NavigationStartingEventArguments.Cancel = true;

                await Windows.System.Launcher.LaunchUriAsync(new Uri(coreWebView2NavigationStartingEventArguments.Uri));
            }
        }
    }
}