using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class DocumentValidatorTests
    {
        [TestMethod]
        public void ValidateDocumentInput_NullDocument_ThrowsError()
        {
            string documentNull = null;
            string documentEmpty = string.Empty;
            string selectedFilePath = "C://";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentNull, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception.Message);
        }

        [TestMethod]
        public void ValidateDocumentInput_EmptyStringDocument_ThrowsError()
        {
            string documentEmpty = string.Empty;
            string selectedFilePath = "C://";
            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentEmpty, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception.Message);
        }

        [TestMethod]
        public void ValidateDocumentInput_NullSelectedFilePath_ThrowsError()
        {
            string selectedFilePathNull = null;
            string document = "Document";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathNull)
            );

            Assert.AreEqual("Please select a file to upload.", exception.Message);
        }

        [TestMethod]
        public void ValidateDocumentInput_EmptyStringSelectedFilePath_ThrowsError()
        {
            string selectedFilePathEmpty = string.Empty;
            string document = "Document";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathEmpty)
            );

            Assert.AreEqual("Please select a file to upload.", exception.Message);
        }

        [TestMethod]
        public void ValidateDocumentInput_CorrectInput_ReturnsTrue()
        {
            string document = "Document";
            string selectedFilePath = "C://";
            Assert.IsTrue(DocumentValidator.ValidateDocumentInput(document, selectedFilePath));
        }
    }
}
