using System;
using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;
using Windows.Storage.Pickers;

namespace PussyCatsApp.Views
{
    /// <summary>
    /// View — responsible only for:
    ///   • rendering data exposed by the ViewModels
    ///   • handling UI events (file picker, Process.Start, dialogs)
    ///   • forwarding user intent to ViewModel methods
    /// </summary>
    public sealed partial class DocumentsListView : Page
    {
        private readonly DocumentListViewModel listViewModel;
        private readonly UploadDocumentViewModel uploadViewModel;
        private static readonly int DefaultUserId = 1; // replace with real session user id

        public DocumentsListView()
        {
            this.InitializeComponent();

            int currentUserId = DefaultUserId;

            var documentRepository = new DocumentRepository(DatabaseConfiguration.GetConnectionString());
            var localStorageService = new LocalFileStorageService();
            var documentService = new DocumentService(documentRepository, localStorageService);

            listViewModel = new DocumentListViewModel(documentService, currentUserId);
            uploadViewModel = new UploadDocumentViewModel(documentService, currentUserId);
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);
            LoadGrid();
        }

        private void LoadGrid()
        {
            listViewModel.LoadDocuments();
            var documents = listViewModel.GetDocuments();

            listViewDocuments.ItemsSource = null;
            listViewDocuments.ItemsSource = documents;

            noDocumentsLabel.Visibility = documents.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void ShowUploadError(string message)
        {
            uploadErrorLabel.Text = message;
            uploadErrorLabel.Visibility = Visibility.Visible;
        }

        private void ShowStatus(string message)
        {
            statusLabel.Text = message;
            statusLabel.Visibility = Visibility.Visible;
        }

        private void OnDocumentNameChanged(object sender, TextChangedEventArgs textChangedEventArguments)
        {
            uploadViewModel.SetDocumentName(txtDocumentName.Text);
        }

        private async void OnBrowseClick(object sender, RoutedEventArgs routedEventArguments)
        {
            var picker = new FileOpenPicker();
            var mainWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, mainWindowHandle);

            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                uploadViewModel.SetSelectedFilePath(file.Path);
                selectedFileLabel.Text = file.Name;
            }
        }

        private void OnUploadClick(object sender, RoutedEventArgs routedEventArguments)
        {
            uploadErrorLabel.Visibility = Visibility.Collapsed;

            try
            {
                uploadViewModel.UploadDocument();

                string error = uploadViewModel.GetErrorMessage();
                if (!string.IsNullOrEmpty(error))
                {
                    ShowUploadError(error);
                    return;
                }

                txtDocumentName.Text = string.Empty;
                selectedFileLabel.Text = "No file selected";
                uploadViewModel.SetDocumentName(string.Empty);
                uploadViewModel.SetSelectedFilePath(null);

                LoadGrid();
            }
            catch (Exception ex)
            {
                ShowUploadError(ex.Message);
                Debug.WriteLine($"[DocumentsView] Upload error: {ex}");
            }
        }

        private async void OnDeleteClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (sender is not Button button || button.Tag is not Document document)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = "Delete Document",
                Content = $"Are you sure you want to delete \"{document.DocumentName}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            try
            {
                listViewModel.DeleteDocument(document.Id);
                LoadGrid();
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message);
            }
        }

        private void OnViewClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (sender is not Button button || button.Tag is not Document document)
            {
                return;
            }

            statusLabel.Visibility = Visibility.Collapsed;

            string fullPath = listViewModel.GetResolvedFilePath(document.Id);

            string status = listViewModel.GetStatusMessage();
            if (!string.IsNullOrEmpty(status))
            {
                ShowStatus(status);
                return;
            }

            if (File.Exists(fullPath))
            {
                Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
            }
            else
            {
                ShowStatus($"The file \"{document.DocumentName}\" could not be found on disk.");
            }
        }

        private void OnBackClick(object sender, RoutedEventArgs routedEventArguments)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}