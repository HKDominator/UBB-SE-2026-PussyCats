using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class DocumentServiceTest
    {
        private Mock<IDocumentRepository> mockDocRepo;
        private Mock<ILocalFileStorageService> mockFileStorage;
        private DocumentService service;

        [TestInitialize]
        public void Setup()
        {
            mockDocRepo = new Mock<IDocumentRepository>();
            mockFileStorage = new Mock<ILocalFileStorageService>();
            service = new DocumentService(mockDocRepo.Object, mockFileStorage.Object);
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
            string tempFile = Path.GetTempFileName();
            string pdfPath = Path.ChangeExtension(tempFile, ".pdf");
            File.Move(tempFile, pdfPath);

           
            mockFileStorage.Setup(s => s.SaveFile(It.IsAny<Stream>(), It.IsAny<string>())).Returns("iss/file.pdf");

            var document = new Document();
            service.UploadDocument(document, pdfPath);

            mockDocRepo.Verify(r => r.AddDocument(document), Times.Once);
            Assert.AreEqual("iss/file.pdf", document.FilePath);
            
            File.Delete(pdfPath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteDocument_DocumentNotFound_ThrowsException()
        {
            //Arrange
            mockDocRepo.Setup(r => r.GetDocumentById(1)).Returns((Document)null);
            //Act
            service.DeleteDocument(1);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithEmptyFilePath_DoesNotCallDeleteFile()
        {
            //Arrange
            mockDocRepo.Setup(r => r.GetDocumentById(1)).Returns(new Document { FilePath = "" });
            //Act
            service.DeleteDocument(1);
            //Assert
            mockFileStorage.Verify(s => s.DeleteFile(It.IsAny<string>()), Times.Never);
            mockDocRepo.Verify(r => r.DeleteDocument(1), Times.Once);
        }

        [TestMethod]
        public void DeleteDocument_DocumentWithFilePath_CallsDeleteFile()
        {
            //Arrange
            mockDocRepo.Setup(r => r.GetDocumentById(1)).Returns(new Document { FilePath = "iss/file.pdf" });
            //Act
            service.DeleteDocument(1);
            //Assert
            mockFileStorage.Verify(s => s.DeleteFile("iss/file.pdf"), Times.Once);
            mockDocRepo.Verify(r => r.DeleteDocument(1), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetDocumentAbsolutePath_DocumentNotFound_ThrowsException()
        {
            //Arrange
            mockDocRepo.Setup(r => r.GetDocumentById(1)).Returns((Document)null);
            //Act
            service.GetDocumentAbsolutePath(1);
        }

        [TestMethod]
        public void GetDocumentAbsolutePath_ValidDocument_ReturnsPath()
        {
            //Arrange
            mockDocRepo.Setup(r => r.GetDocumentById(1)).Returns(new Document { FilePath = "iss/file.pdf" });
            mockFileStorage.Setup(s => s.GetFilePath("iss/file.pdf")).Returns("C:/Downloads/iss/file.pdf");
            //Act
            var result = service.GetDocumentAbsolutePath(1);
            //Assert
            Assert.AreEqual("C:/Downloads/iss/file.pdf", result);
        }
    }
}
