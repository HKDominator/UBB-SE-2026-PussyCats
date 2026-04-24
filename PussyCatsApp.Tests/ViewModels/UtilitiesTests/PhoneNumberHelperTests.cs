using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class PhoneNumberHelperTests
    {
        [TestMethod]
        public void ExtractPhonePrefixAndNumber_NullPhoneNumber_ReturnsEmptyPrefixAndNumber()
        {
            Assert.AreEqual((string.Empty, string.Empty), PhoneNumberHelper.ExtractPhonePrefixAndNumber(null));
        }

        [TestMethod]
        public void ExtractPhonePrefixAndNumber_EmptyPhoneNumber_ReturnsEmptyPrefixAndNumber()
        {
            Assert.AreEqual((string.Empty, string.Empty), PhoneNumberHelper.ExtractPhonePrefixAndNumber(string.Empty));
        }

        [TestMethod]
        public void ExtractPhonePrefixAndNumber_PrefixMatches_ReturnsPrefixAndNumber()
        {
            var result = PhoneNumberHelper.ExtractPhonePrefixAndNumber("+40123456789");
            Assert.AreEqual("+40", result.prefix);
            Assert.AreEqual("123456789", result.number);
        }

        [TestMethod]
        public void ExtractPhonePrefixAndNumber_PrefixDoesNotMatch_ReturnsEmptyPrefix()
        {
            var result = PhoneNumberHelper.ExtractPhonePrefixAndNumber("123456789");
            Assert.AreEqual(string.Empty, result.prefix);
            Assert.AreEqual("123456789", result.number);
        }
    }
}
