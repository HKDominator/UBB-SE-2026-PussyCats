using System.Text;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class LocalFileStorageServiceTests
    {
        private string temporaryDirectory;
        private LocalFileStorageService localFileStorageService;
        
        private const int SmallFileSizeInBytes = 64;
        private const int StandardFileSizeInBytes = 256;

        [TestInitialize]
        public void SetUp()
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temporaryDirectory);
            localFileStorageService = new LocalFileStorageService(temporaryDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(temporaryDirectory))
                Directory.Delete(temporaryDirectory, recursive: true);
        }

        [TestMethod]
        [DataRow("cv.pdf")]
        public void SaveFile_ValidFile_CreatesFileOnDisk(string fileName)
        {
            using var validPdfUploadStream = new MemoryStream(new byte[StandardFileSizeInBytes]);

            string savedPath = localFileStorageService.SaveFile(validPdfUploadStream, fileName);

            Assert.IsTrue(File.Exists(savedPath));
        }

        [TestMethod]
        public void SaveFile_ValidFile_CreatedFileContentsMatch()
        {
            byte[] expectedFileContent = { 1, 2, 3, 4, 5, 123, 0, 12 };
            using var fileUploadStream = new MemoryStream(expectedFileContent);

            string savedPath = localFileStorageService.SaveFile(fileUploadStream, "file.pdf");

            Assert.IsTrue(File.Exists(savedPath));
            Assert.IsTrue(File.ReadAllBytes(savedPath).SequenceEqual(expectedFileContent));
        }

        [TestMethod]
        public void SaveFile_TwoFilesWithSameName_HaveUniqueFilePaths()
        {
            using var firstFileUploadStream = new MemoryStream(new byte[SmallFileSizeInBytes]);
            using var secondFileUploadStream = new MemoryStream(new byte[SmallFileSizeInBytes]);

            string path1 = localFileStorageService.SaveFile(firstFileUploadStream, "file.pdf");
            string path2 = localFileStorageService.SaveFile(secondFileUploadStream, "file.pdf");

            Assert.AreNotEqual(path1, path2);
        }


        [TestMethod]
        public void DeleteFile_ExistingFile_RemovesFileFromDisk()
        {
            using var fileUploadStream = new MemoryStream(new byte[SmallFileSizeInBytes]);
            string savedFilePath = localFileStorageService.SaveFile(fileUploadStream, "file.pdf");
            Assert.IsTrue(File.Exists(savedFilePath));

            localFileStorageService.DeleteFile(savedFilePath);

            Assert.IsFalse(File.Exists(savedFilePath));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void DeleteFile_NullOrWhitespacePath_DoesNotThrow(string? path)
        {
            localFileStorageService.DeleteFile(path);
        }

        [TestMethod]
        public void DeleteFile_NonExistingFile_DoesNotThrow()
        {
            string nonExistent = Path.Combine(temporaryDirectory, "doesNotExist.pdf");
            localFileStorageService.DeleteFile(nonExistent);
        }


        [TestMethod]
        public void GetFilePath_ExistingFile_ReturnsValidPath()
        {
            using var stream = new MemoryStream(new byte[SmallFileSizeInBytes]);
            string savedPath = localFileStorageService.SaveFile(stream, "somecv.pdf");
            string retrievedPath = localFileStorageService.GetFilePath(savedPath);
            Assert.AreEqual(savedPath, retrievedPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilePath_NullPath_ThrowsArgumentNullException()
        {
            localFileStorageService.GetFilePath(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetFilePath_InvalidPath_ThrowsFileNotFound()
        {
            localFileStorageService.GetFilePath("lalala.pdf");
        }
    }
}