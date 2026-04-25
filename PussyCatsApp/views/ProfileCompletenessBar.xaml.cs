using Microsoft.UI.Xaml.Controls;
namespace PussyCatsApp.Views
{
    /// <summary>
    /// UserControl for displaying a profile completeness progress bar and prompt text.
    /// </summary>
    public sealed partial class ProfileCompletenessBar : UserControl
    {
        public ProfileCompletenessBar()
        {
            this.InitializeComponent();
        }
        public void Update(int percentage, string promptText)
        {
            barProgress.Value = percentage;
            percentageLabel.Text = $"{percentage}%";
            promptLabel.Text = promptText;
        }
    }
}