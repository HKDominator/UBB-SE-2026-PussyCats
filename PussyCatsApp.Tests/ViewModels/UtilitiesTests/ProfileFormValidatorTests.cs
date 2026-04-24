using PussyCatsApp.Models;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class ProfileFormValidatorTests
    {
        [TestMethod]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            string validEmail1 = "validemail@gmail.com";
            Assert.AreEqual(true, ProfileFormValidator.IsValidEmail(validEmail1));
        }

        [TestMethod]
        public void IsValidEmail_InvalidEmailNoDotNoAt_ReturnsFalse()
        {
            string invalidEmail = "invalidemail";
            Assert.AreEqual(false, ProfileFormValidator.IsValidEmail(invalidEmail));
        }

        [TestMethod]
        public void IsValidEmail_InvalidEmailNoDot_ReturnsFalse()
        {
            string invalidEmail = "invalidemail@";
            Assert.IsFalse(ProfileFormValidator.IsValidEmail(invalidEmail));
        }

        [TestMethod]
        public void IsValidEmail_InvalidEmailNoAt_ReturnsFalse()
        {
            string invalidEmail = "invalidemail.com";
            Assert.IsFalse(ProfileFormValidator.IsValidEmail(invalidEmail));
        }

        [TestMethod]
        public void IsValidEmail_InvalidEmailEmptyBetweenDotAndAt_ReturnsFalse()
        {
            string invalidEmail = "invalidemail@.com";
            Assert.IsFalse(ProfileFormValidator.IsValidEmail(invalidEmail));
        }

        [TestMethod]
        public void ValidateForm_AllButLastNameAreInvalidEntries_ReturnsEveryErrorExceptLastName()
        {
            string firstName = string.Empty;
            string lastName = "Doe";
            double age = 0;
            string gender = string.Empty;
            string email = string.Empty;
            string phonePrefix = string.Empty;
            string phoneNumber = string.Empty;
            string country = string.Empty;
            string city = string.Empty;
            string university = string.Empty;
            int expectedGraduationYear = 0;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            List<string> errors = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            // phone prefix and phone number are combined into one error message so they count as one error instead of two
            int numberOfExpectedErrors = 9;

            Assert.AreEqual(numberOfExpectedErrors, errors.Count);
            Assert.IsFalse(errors.Contains("Last Name"));
        }

        [TestMethod]
        public void ValidateForm_AllButAgeAreInvalid_ReturnsEveryErrorExceptAge()
        {
            string firstName = string.Empty;
            string lastName = string.Empty;
            double age = 25;
            string gender = string.Empty;
            string email = string.Empty;
            string phonePrefix = string.Empty;
            string phoneNumber = string.Empty;
            string country = string.Empty;
            string city = string.Empty;
            string university = string.Empty;
            int expectedGraduationYear = 0;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            List<string> errors = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            // phone prefix and phone number are combined into one error message so they count as one error instead of two
            int numberOfExpectedErrors = 9;

            double minimumAge = 16, maximumAge = 60;
            Assert.AreEqual(numberOfExpectedErrors, errors.Count);
            Assert.IsFalse(errors.Contains($"Age (must be between {minimumAge}-{maximumAge})"));
        }

        [TestMethod]
        public void ValidateForm_InvalidLastName_ReturnsErrorLastName()
        {
            string firstName = "John";
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            string lastName = string.Empty;
            double age = 25;

            List<string> errors = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            int numberOfExpectedErrors = 1;

            Assert.AreEqual(numberOfExpectedErrors, errors.Count);
            Assert.IsTrue(errors.Contains("Last Name"));
        }

        [TestMethod]
        public void ValidateForm_UnderageAge_ReturnsError()
        {
            string firstName = "John";
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            string lastName = "Doe";

            double minimumAge = 16, maximumAge = 60;
            double ageUnderLimit = 10;
            List<string> errorsUnderage = ProfileFormValidator.ValidateForm(firstName, lastName, ageUnderLimit, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            int numberOfExpectedErrors = 1;

            Assert.AreEqual(numberOfExpectedErrors, errorsUnderage.Count);
            Assert.IsTrue(errorsUnderage.Contains($"Age (must be between {minimumAge}-{maximumAge})"));
        }

        [TestMethod]
        public void ValidateForm_OverageAge_ReturnsError()
        {
            string firstName = "John";
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();
            string lastName = "Doe";

            double minimumAge = 16, maximumAge = 60;
            int numberOfExpectedErrors = 1;

            double ageOverLimit = 100;
            List<string> errorsOverAge = ProfileFormValidator.ValidateForm(firstName, lastName, ageOverLimit, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            Assert.AreEqual(numberOfExpectedErrors, errorsOverAge.Count);
            Assert.IsTrue(errorsOverAge.Contains($"Age (must be between {minimumAge}-{maximumAge})"));
        }

            [TestMethod]
        public void ValidateForm_StartDateAfterEndDateExperiences_ReturnsError()
        {
            // Arrange
            string firstName = "John";
            string lastName = "Doe";
            double age = 25;
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();

            WorkExperience workExperienceWrong = new WorkExperience
            {
                Company = "Company A",
                JobTitle = "Developer",
                StartDate = new DateTimeOffset(new DateTime(2020, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2019, 1, 1)),
                Description = "Worked on various projects.",
                CurrentlyWorking = false
            };

            

            workExperiences.Add(workExperienceWrong);

            List<string> errorsWithWrongWorkExperience = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);

            Assert.AreEqual(1, errorsWithWrongWorkExperience.Count);
            Assert.IsTrue(errorsWithWrongWorkExperience.Contains($"Work Experience \"{workExperienceWrong.Company}\": End date is before start date"));
        }

        [TestMethod]
        public void ValidateForm_CurrentlyWorking_ReturnsZeroErrors()
        {
            string firstName = "John";
            string lastName = "Doe";
            double age = 25;
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();

            WorkExperience workExperienceCurrentlyWorking = new WorkExperience
            {
                Company = "Company A",
                JobTitle = "Developer",
                StartDate = new DateTimeOffset(new DateTime(2019, 1, 1)),
                EndDate = null,
                Description = "Worked on various projects.",
                CurrentlyWorking = true
            };

            workExperiences.Add(workExperienceCurrentlyWorking);

            List<string> errorsWithWorkExperienceCurrentlyWorking = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            Assert.AreEqual(0, errorsWithWorkExperienceCurrentlyWorking.Count); /// Everything should be fine
        }

        [TestMethod]
        public void ValidateForm_EndDateAfterStartDate_ReturnsZeroErrors()
        {
            string firstName = "John";
            string lastName = "Doe";
            double age = 25;
            string gender = "Male";
            string email = "validemail@gmail.com";
            string phonePrefix = "+40";
            string phoneNumber = "12345";
            string country = "Romania";
            string city = "Cluj-Napoca";
            string university = "UBB";
            int expectedGraduationYear = 2027;
            List<WorkExperience> workExperiences = new List<WorkExperience>();

            WorkExperience workExperienceRight = new WorkExperience
            {
                Company = "Company A",
                JobTitle = "Developer",
                StartDate = new DateTimeOffset(new DateTime(2019, 1, 1)),
                EndDate = new DateTimeOffset(new DateTime(2020, 1, 1)),
                Description = "Worked on various projects.",
                CurrentlyWorking = false
            };

            workExperiences.Add(workExperienceRight);

            List<string> errorsWithWorkExperienceCurrentlyWorking = ProfileFormValidator.ValidateForm(firstName, lastName, age, gender, email, phonePrefix, phoneNumber, country, city, university, expectedGraduationYear, workExperiences);
            Assert.AreEqual(0, errorsWithWorkExperienceCurrentlyWorking.Count); /// Everything should be fine
        }
    }
}
