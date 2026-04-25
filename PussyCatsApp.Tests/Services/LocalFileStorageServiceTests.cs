using System.Text;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class LocalFileStorageServiceTests
    {
        private string tempDir;
        private LocalFileStorageService service;

        [TestInitialize]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            service = new LocalFileStorageService(tempDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }

        [TestMethod]
        [DataRow("cv.pdf")]
        public void SaveFile_ValidFile_CreatesFileOnDisk(string fileName)
        {
            using var validPdfUploadStream = new MemoryStream(new byte[256]);

            string savedPath = service.SaveFile(validPdfUploadStream, fileName);

            Assert.IsTrue(File.Exists(savedPath));
        }

        [TestMethod]
        public void SaveFile_ValidFile_CreatedFileContentsMatch()
        {
            byte[] expectedFileContent = { 1, 2, 3, 4, 5, 123, 0, 12 };
            using var fileUploadStream = new MemoryStream(expectedFileContent);

            string savedPath = service.SaveFile(fileUploadStream, "file.pdf");

            Assert.IsTrue(File.Exists(savedPath));
            Assert.IsTrue(File.ReadAllBytes(savedPath).SequenceEqual(expectedFileContent));
        }

        [TestMethod]
        public void SaveFile_TwoFilesWithSameName_HaveUniqueFilePaths()
        {
            using var firstFileUploadStream = new MemoryStream(new byte[64]);
            using var secondFileUploadStream = new MemoryStream(new byte[64]);

            string path1 = service.SaveFile(firstFileUploadStream, "file.pdf");
            string path2 = service.SaveFile(secondFileUploadStream, "file.pdf");

            Assert.AreNotEqual(path1, path2);
        }


        [TestMethod]
        public void DeleteFile_ExistingFile_RemovesFileFromDisk()
        {
            using var fileUploadStream = new MemoryStream(new byte[64]);
            string savedFilePath = service.SaveFile(fileUploadStream, "file.pdf");
            Assert.IsTrue(File.Exists(savedFilePath));

            service.DeleteFile(savedFilePath);

            Assert.IsFalse(File.Exists(savedFilePath));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void DeleteFile_NullOrWhitespacePath_DoesNotThrow(string? path)
        {
            service.DeleteFile(path);
        }

        [TestMethod]
        public void DeleteFile_NonExistingFile_DoesNotThrow()
        {
            string nonExistent = Path.Combine(tempDir, "doesNotExist.pdf");
            service.DeleteFile(nonExistent);
        }


        [TestMethod]
        public void GetFilePath_ExistingFile_ReturnsValidPath()
        {
            using var stream = new MemoryStream(new byte[64]);
            string savedPath = service.SaveFile(stream, "somecv.pdf");
            string retrievedPath = service.GetFilePath(savedPath);
            Assert.AreEqual(savedPath, retrievedPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilePath_NullPath_ThrowsArgumentNullException()
        {
            service.GetFilePath(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetFilePath_InvalidPath_ThrowsFileNotFound()
        {
            service.GetFilePath("lalala.pdf");
        }
    }
}