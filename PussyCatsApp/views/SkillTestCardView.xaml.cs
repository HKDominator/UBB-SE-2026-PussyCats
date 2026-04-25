using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using PussyCatsApp.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace PussyCatsApp.Views
{
    /// <summary>
    /// UserControl for displaying a skill test card, including test details, badge, and retake functionality.
    /// </summary>
    public sealed partial class SkillTestCardView : UserControl
    {
        private SkillTestCardViewModel skillTestCardViewModel;
        private static readonly int BadgeIconRasterizePixelSize = 100;
        private static readonly double RetakeButtonEnabledOpacity = 1.0;
        private static readonly double RetakeButtonDisabledOpacity = 0.4;

        public SkillTestCardView(SkillTestCardViewModel viewModel)
        {
            this.InitializeComponent();
            this.skillTestCardViewModel = viewModel;
            this.DataContext = skillTestCardViewModel;
            BadgeIcon.ImageFailed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"IMAGE ERROR: {e.ErrorMessage}");
            };
            LoadCard();
        }

        private void LoadCard()
        {
            TestNameText.Text = skillTestCardViewModel.SkillTest.Name?.ToUpper() + " TEST" ?? "UNKNOWN TEST";
            string scoreDisplay = $"SCORE: {skillTestCardViewModel.SkillTest.Score}%";
            ScoreText.Text = scoreDisplay;
            DateText.Text = SkillTestService.AchievedDateFormatted(skillTestCardViewModel.SkillTest);

            if (skillTestCardViewModel.Badge != null && !string.IsNullOrEmpty(skillTestCardViewModel.Badge.IconPath))
            {
                string path = skillTestCardViewModel.Badge.IconPath;

                if (!path.StartsWith("ms-appx:///"))
                {
                    path = $"ms-appx:///{path.TrimStart('/')}";
                }

                var uri = new Uri(path);
                System.Diagnostics.Debug.WriteLine($"FIXED URI: {uri}");

                var svgSource = new SvgImageSource(uri);
                svgSource.RasterizePixelWidth = BadgeIconRasterizePixelSize;
                svgSource.RasterizePixelHeight = BadgeIconRasterizePixelSize;

                BadgeIcon.Source = svgSource;
            }
            UpdateRetakeButton();
        }

        private void UpdateRetakeButton()
        {
            RetakeButton.IsEnabled = skillTestCardViewModel.IsRetakeEnabled;
            RetakeButton.Opacity = skillTestCardViewModel.IsRetakeEnabled ? RetakeButtonEnabledOpacity : RetakeButtonDisabledOpacity;
        }

        private void RetakeButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            skillTestCardViewModel.RetakeCommand();
            LoadCard();
        }
    }
}
