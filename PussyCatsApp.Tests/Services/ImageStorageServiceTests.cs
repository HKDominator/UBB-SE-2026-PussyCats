using System.Text;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{

    [TestClass]
    public class ImageStorageServiceTests
    {
        private string temporaryDirectory;
        private ImageStorageService imageStorageService;
        private const int BytesInKilobyte = 1024;
        private const int BytesInMegabyte = BytesInKilobyte * BytesInKilobyte;
        private const int MaxAllowedImageSizeInMB = 5;
        private const int SmallImageSizeInBytes = 1024;
        private const int TinyImageSizeInBytes = 64;



        [TestInitialize]
        public void SetUp()
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temporaryDirectory);

            imageStorageService = new ImageStorageService(temporaryDirectory);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(temporaryDirectory))
                Directory.Delete(temporaryDirectory, recursive: true);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("image.bmp")]
        [DataRow("image.gif")]
        [DataRow("image.txt")]
        [DataRow("image.pdf")]
        [DataRow("image.exe")]
        [DataRow("image.zip")]
        [DataRow("image.docx")]
        [DataRow("image.hellojbmn")]
        public void SaveImage_UnsupportedFileType_ThrowsArgumentException(string fileName)
        {
            
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake image data"));

            imageStorageService.SaveImage(fileStream, fileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("image")]
        public void SaveImage_NoFileExtension_ThrowsArgumentException(string fileName)
        {
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake image data"));

            imageStorageService.SaveImage(fileStream, fileName);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SaveImage_FileSizeTooBig_ThrowsException()
        {
            var fakeImage = new byte[(MaxAllowedImageSizeInMB + 1) * BytesInMegabyte];
            var fileStream = new MemoryStream(fakeImage);

            imageStorageService.SaveImage(fileStream,"myFileName.png");

        }

        [TestMethod]
        [DataRow("photo.jpg")]
        [DataRow("photo.png")]
        [DataRow("photo.jpeg")]
        [DataRow("photo.JPG")]
        [DataRow("photo.PNG")]
        [DataRow("photo.JPEG")]
        [DataRow("photo.JpG")]
        public void SaveImage_ValidImage_CreatesFileOnDisk(string fileName)
        {
            byte[] fakeImage = new byte[SmallImageSizeInBytes];
            using var stream = new MemoryStream(fakeImage);

            string savedPath = imageStorageService.SaveImage(stream, fileName);

            Assert.IsTrue(File.Exists(savedPath));
        }

        [TestMethod]
        public void SaveImage_ValidImage_CreatedFileContentsMatch()
        {
            byte[] fakeImage = { 1, 2, 3, 4, 5, 123, 0, 12 };
            using var stream = new MemoryStream(fakeImage);

            string savedPath = imageStorageService.SaveImage(stream, "fileName.png");

            Assert.IsTrue(File.Exists(savedPath));
            Assert.IsTrue(File.ReadAllBytes(savedPath).SequenceEqual(fakeImage));

        }

        [TestMethod]
        public void SaveImage_TwoImagesSameFileName_HaveUniqueFilePaths()
        {

            using var firstImageStream = new MemoryStream(new byte[TinyImageSizeInBytes]);
            using var secondImageStream = new MemoryStream(new byte[TinyImageSizeInBytes]);

            string path1 = imageStorageService.SaveImage(firstImageStream, "a.jpg");
            string path2 = imageStorageService.SaveImage(secondImageStream, "a.jpg");

            Assert.AreNotEqual(path1, path2);
            
        }


        [TestMethod]
        public void DeleteImage_ExistingFile_RemovesFileFromDisk()
        {
            using var stream = new MemoryStream(new byte[TinyImageSizeInBytes]);
            string savedPath = imageStorageService.SaveImage(stream, "photo.jpg");
            Assert.IsTrue(File.Exists(savedPath));

            imageStorageService.DeleteImage(savedPath);

            Assert.IsFalse(File.Exists(savedPath));
        }

        [TestMethod]
        public void DeleteImage_NullOrWhitespacePath_DoesNotThrow()
        {
            imageStorageService.DeleteImage("   ");
        }

        [TestMethod]
        public void DeleteImage_NonExistingFile_DoesNotThrow()
        {
            string nonExistingPath = Path.Combine(temporaryDirectory, "nonexistent.jpg");
            imageStorageService.DeleteImage(nonExistingPath);
        }
    }
}
