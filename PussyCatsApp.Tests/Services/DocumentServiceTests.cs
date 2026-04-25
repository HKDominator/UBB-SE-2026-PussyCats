using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

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
            mockDocumentRepo.Setup(doesNotFindDocument => doesNotFindDocument.GetDocumentById(1)).Returns((Document)null);
            //Act
            service.DeleteDocument(1);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithEmptyFilePath_DoesNotCallDeleteFile()
        {
            //Arrange
            mockDocumentRepo.Setup(doesNotHaveFilePath => doesNotHaveFilePath.GetDocumentById(1)).Returns(new Document { FilePath = "" });
            //Act
            service.DeleteDocument(1);
            //Assert
            mockFileStorage.Verify(doesNotDeleteDocumentWithNoFilePath => doesNotDeleteDocumentWithNoFilePath.DeleteFile(It.IsAny<string>()), Times.Never);
            mockDocumentRepo.Verify(deletesDocumentEntry => deletesDocumentEntry.DeleteDocument(1), Times.Once);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithFilePath_CallsDeleteFile()
        {
            //Arrange
            mockDocumentRepo.Setup(findsDocumentWithIssFilePathPdf => findsDocumentWithIssFilePathPdf.GetDocumentById(1)).Returns(new Document { FilePath = "iss/file.pdf" });
            //Act
            service.DeleteDocument(1);
            //Assert
            mockFileStorage.Verify(fileStorageDeletesExistingFile => fileStorageDeletesExistingFile.DeleteFile("iss/file.pdf"), Times.Once);
            mockDocumentRepo.Verify(documentRepositoryDeletesEntry => documentRepositoryDeletesEntry.DeleteDocument(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetDocumentAbsolutePath_DocumentNotFound_ThrowsException()
        {
            //Arrange
            mockDocumentRepo.Setup(doesNotFindDocument => doesNotFindDocument.GetDocumentById(1)).Returns((Document)null);
            //Act
            service.GetDocumentAbsolutePath(1);
        }

        [TestMethod]
        public void GetDocumentAbsolutePath_ValidDocument_ReturnsPath()
        {
            //Arrange
            mockDocumentRepo.Setup(findsIssFilePathPdf => findsIssFilePathPdf.GetDocumentById(1)).Returns(new Document { FilePath = "iss/file.pdf" });
            mockFileStorage.Setup(resolvesIssFilePathToAbsolutePath => resolvesIssFilePathToAbsolutePath.GetFilePath("iss/file.pdf")).Returns("C:/Downloads/iss/file.pdf");
            //Act
            var result = service.GetDocumentAbsolutePath(1);
            //Assert
            Assert.AreEqual("C:/Downloads/iss/file.pdf", result);
        }
    }
}
