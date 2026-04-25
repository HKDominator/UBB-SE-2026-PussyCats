using System.Collections.Generic;
using PussyCatsApp.Services;
using PussyCatsApp.Models;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for the document list panel.
    /// Holds data and delegates to the service
    /// Opening the file with the default application is the View's responsibility:
    /// the ViewModel exposes GetResolvedFilePath() so the View can act on it.
    /// </summary>
    public class DocumentListViewModel
    {
        private List<Document> documents;
        private string statusMessage;
        private readonly int userId;
        private readonly IDocumentService documentService;

        public DocumentListViewModel(IDocumentService documentService, int userId)
        {
            this.documentService = documentService;
            this.userId = userId;
            this.documents = new List<Document>();
        }

        public void LoadDocuments()
        {
            documents = documentService.GetDocumentsByUserId(userId);
        }

        public List<Document> GetDocuments() => documents;

        public void DeleteDocument(int documentId)
        {
            documentService.DeleteDocument(documentId);
            LoadDocuments();
        }

        /// <summary>
        /// Resolves the absolute path for a document.
        /// Returns null and sets a status message if the document is not found.
        /// The View is responsible for checking the path exists and launching
        /// the OS default application
        /// </summary>
        public string GetResolvedFilePath(int documentId)
        {
            try
            {
                string fullPath = documentService.GetDocumentAbsolutePath(documentId);
                statusMessage = string.Empty;
                return fullPath;
            }
            catch
            {
                statusMessage = "The file could not be found on disk.";
                return null;
            }
        }

        public string GetStatusMessage() => statusMessage;
    }
}