using System.Reflection.Metadata;
using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using Document = PussyCatsApp.Models.Document;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class DocumentServiceTest
    {
        private Mock<IDocumentRepository> mockDocumentRepo;
        private Mock<ILocalFileStorageService> mockFileStorage;
        private DocumentService service;

        [TestInitialize]
        public void Setup()
        {
            mockDocumentRepo = new Mock<IDocumentRepository>();
            mockFileStorage = new Mock<ILocalFileStorageService>();
            service = new DocumentService(mockDocumentRepo.Object, mockFileStorage.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UploadDocument_InvalidFileType_ThrowsException()
        {
            //Act
            service.UploadDocument(new Document(), "file.exe");
        }
        [TestMethod]
        public void UploadDocument_ValidPdfFile_CallsAddDocument()
        {
            string temporaryFile = Path.GetTempFileName();
            string pdfPath = Path.ChangeExtension(temporaryFile, ".pdf");
            File.Move(temporaryFile, pdfPath);

           
            mockFileStorage.Setup(issFilePdfPath => issFilePdfPath.SaveFile(It.IsAny<Stream>(), It.IsAny<string>())).Returns("iss/file.pdf");

            var document = new Document();
            service.UploadDocument(document, pdfPath);


            mockDocumentRepo.Verify(issFilePdfAdd => issFilePdfAdd.AddDocument(document), Times.Once);

            Assert.AreEqual("iss/file.pdf", document.FilePath);
            
            File.Delete(pdfPath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteDocument_DocumentNotFound_ThrowsException()
        {
            //Arrange
            int documentId = 1;
            mockDocumentRepo.Setup(doesNotFindDocument => doesNotFindDocument.GetDocumentById(documentId)).Returns((Document)null);
            //Act
            service.DeleteDocument(1);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithEmptyFilePath_DoesNotCallDeleteFile()
        {
            //Arrange
            int documentId = 1;
            mockDocumentRepo.Setup(doesNotHaveFilePath => doesNotHaveFilePath.GetDocumentById(documentId)).Returns(new Document { FilePath = "" });
            //Act
            service.DeleteDocument(documentId);
            //Assert
            mockFileStorage.Verify(doesNotDeleteDocumentWithNoFilePath => doesNotDeleteDocumentWithNoFilePath.DeleteFile(It.IsAny<string>()), Times.Never);
            mockDocumentRepo.Verify(deletesDocumentEntry => deletesDocumentEntry.DeleteDocument(documentId), Times.Once);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithFilePath_CallsDeleteFile()
        {
            //Arrange
            int documentId = 1;
            mockDocumentRepo.Setup(findsDocumentWithIssFilePathPdf => findsDocumentWithIssFilePathPdf.GetDocumentById(documentId)).Returns(new Document { FilePath = "iss/file.pdf" });
            //Act
            service.DeleteDocument(documentId);
            //Assert
            mockFileStorage.Verify(fileStorageDeletesExistingFile => fileStorageDeletesExistingFile.DeleteFile("iss/file.pdf"), Times.Once);
            mockDocumentRepo.Verify(documentRepositoryDeletesEntry => documentRepositoryDeletesEntry.DeleteDocument(documentId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetDocumentAbsolutePath_DocumentNotFound_ThrowsException()
        {
            //Arrange
            int documentId = 1;
            mockDocumentRepo.Setup(doesNotFindDocument => doesNotFindDocument.GetDocumentById(documentId)).Returns((Document)null);

            //Act
            service.GetDocumentAbsolutePath(documentId);
        }

        [TestMethod]
        public void GetDocumentAbsolutePath_ValidDocument_ReturnsPath()
        {
            //Arrange
            int documentId = 1;
            mockDocumentRepo.Setup(findsIssFilePathPdf => findsIssFilePathPdf.GetDocumentById(documentId)).Returns(new Document { FilePath = "iss/file.pdf" });
            mockFileStorage.Setup(resolvesIssFilePathToAbsolutePath => resolvesIssFilePathToAbsolutePath.GetFilePath("iss/file.pdf")).Returns("C:/Downloads/iss/file.pdf");

            //Act
            var result = service.GetDocumentAbsolutePath(documentId);
            //Assert
            Assert.AreEqual("C:/Downloads/iss/file.pdf", result);
        }
    }
}
