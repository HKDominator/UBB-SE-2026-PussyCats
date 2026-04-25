using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class CVParsingService : ICVParsingService
    {
        private const string JsonExtension = ".json";
        private const string XmlExtension = ".xml";
        private const string ParseErrorMessage = "Failed to parse CV file: ";
        private const string UnsupportedTypeMessage = "Unsupported file type. Only JSON and XML are supported.";

        private const int MaxSkills = 30;
        private const int MaxSkillLength = 60;

        private const int MaxFirstNameLength = 50;
        private const int MaxLastNameLength = 60;
        private const int MaxCountryLength = 100;
        private const int MaxCityLength = 100;
        private const int MaxUniversityLength = 200;
        private const int MaxGitHubLength = 200;
        private const int MaxLinkedInLength = 200;
        private const int MaxAddressLength = 500;
        private const int MaxMotivationLength = 1000;
        private const int MaxEmailLength = 254;
        private const int MaxPhoneLength = 15;
        private const int MaxCompanyNameLength = 150;
        private const int MaxJobTitleLength = 100;
        private const int MaxWorkDescriptionLength = 500;

        private const int MinAge = 16;
        private const int MaxAge = 60;
        private const int InvalidAge = 0;
        private const int MaxYearsAheadForGraduation = 10;

        private const string GenderMale = "Male";
        private const string GenderFemale = "Female";
        private const string GenderMaleShort = "M";
        private const string GenderFemaleShort = "F";

        private static readonly DateTime MinValidDate = new DateTime(1980, 1, 1);
        private const int MaxYearsAheadForDate = 1;

        /// <summary>
        /// Parses a CV file (JSON or XML) and returns a UserProfile object
        /// </summary>
        public UserProfile ParseCVFile(string content, string fileType)
        {
            try
            {
                if (string.Equals(fileType, JsonExtension, StringComparison.OrdinalIgnoreCase))
                {
                    var cvData = JsonSerializer.Deserialize<CVData>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return MapCVDataToProfile(cvData);
                }

                if (string.Equals(fileType, XmlExtension, StringComparison.OrdinalIgnoreCase))
                {
                    var xmlDocument = XDocument.Parse(content);
                    var rootName = xmlDocument.Root?.Name.LocalName ?? "CVData";

                    using var reader = new StringReader(content);

                    var serializer = new XmlSerializer(typeof(CVData), new XmlRootAttribute(rootName));
                    var cvData = (CVData)serializer.Deserialize(reader);

                    return MapCVDataToProfile(cvData);
                }

                throw new Exception(UnsupportedTypeMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ParseErrorMessage + ex.Message, ex);
            }
        }

        /// <summary>
        /// Maps CV data structure to UserProfile
        /// </summary>
        private UserProfile MapCVDataToProfile(CVData cvData)
        {
            if (cvData == null)
            {
                return new UserProfile();
            }

            return new UserProfile
            {
                FirstName = SanitizeString(cvData.FirstName, MaxFirstNameLength),
                LastName = SanitizeString(cvData.LastName, MaxLastNameLength),
                Age = ValidateAge(cvData.Age),
                Gender = ValidateGender(cvData.Gender),
                Email = SanitizeEmail(cvData.Email),
                PhoneNumber = FormatPhoneNumber(cvData.PhoneNumber),
                Country = SanitizeString(cvData.Country, MaxCountryLength),
                City = SanitizeString(cvData.City, MaxCityLength),
                University = SanitizeString(cvData.University, MaxUniversityLength),
                ExpectedGraduationYear = ValidateGraduationYear(cvData.ExpectedGraduationYear),

                GitHub = SanitizeString(cvData.GitHub, MaxGitHubLength),
                LinkedIn = SanitizeString(cvData.LinkedIn, MaxLinkedInLength),
                Address = SanitizeString(cvData.Address, MaxAddressLength),
                Motivation = SanitizeString(cvData.Motivation, MaxMotivationLength),

                Skills = ProcessSkills(cvData.Skills),
                WorkExperiences = ProcessWorkExperiences(cvData.WorkExperiences),
                Projects = ProcessProjects(cvData.Projects),
                ExtraCurricularActivities = ProcessActivities(cvData.ExtraCurricularActivities)
            };
        }

        /// <summary>
        /// Processes and validates skills, removing duplicates
        /// </summary>
        private List<string> ProcessSkills(List<string> skills)
        {
            var result = new List<string>();

            if (skills == null)
            {
                return result;
            }

            var addedSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int addedSkillsCount = 0;

            foreach (var skill in skills)
            {
                if (addedSkillsCount >= MaxSkills)
                {
                    break;
                }

                var sanitized = SanitizeString(skill, MaxSkillLength);

                if (!string.IsNullOrWhiteSpace(sanitized) && addedSkills.Add(sanitized))
                {
                    result.Add(sanitized);
                    addedSkillsCount++;
                }
            }

            return result;
        }

        /// <summary>
        /// Processes work experiences with validation
        /// </summary>
        private List<WorkExperience> ProcessWorkExperiences(List<WorkExperience> experiences)
        {
            if (experiences == null || !experiences.Any())
            {
                return new List<WorkExperience>();
            }
            const int maximumNumberOfWorkExperiences = 10;

            return experiences.Take(maximumNumberOfWorkExperiences)
                .Select(workExperience => new WorkExperience
                {
                    Company = SanitizeString(workExperience.Company, MaxCompanyNameLength),
                    JobTitle = SanitizeString(workExperience.JobTitle, MaxJobTitleLength),
                    StartDate = ValidateDate(workExperience.StartDate),
                    EndDate = workExperience.CurrentlyWorking ? null : ValidateDate(workExperience.EndDate),
                    CurrentlyWorking = workExperience.CurrentlyWorking,
                    Description = SanitizeString(workExperience.Description, MaxWorkDescriptionLength)
                })
                .Where(workExperience => !string.IsNullOrEmpty(workExperience.Company) && !string.IsNullOrEmpty(workExperience.JobTitle))
                .ToList();
        }

        /// <summary>
        /// Processes projects with validation
        /// </summary>
        private List<Project> ProcessProjects(List<Project> projects)
        {
            if (projects == null || !projects.Any())
            {
                return new List<Project>();
            }
            const int maximumNumberOfProjects = 10;
            const int maximumNumberOfTechnologiesPerProject = 10;
            const int maximumProjectNameLength = 100;
            const int maximumProjectDescriptionLength = 600;
            const int maximumProjectUrlLength = 200;
            const int maximumTechnologyNameLength = 60;

            return projects.Take(maximumNumberOfProjects)
                .Select(project => new Project
                {
                    Name = SanitizeString(project.Name, maximumProjectNameLength),
                    Description = SanitizeString(project.Description, maximumProjectDescriptionLength),
                    Technologies = project.Technologies?.Take(maximumNumberOfTechnologiesPerProject).Select(technology => SanitizeString(technology, maximumTechnologyNameLength)).ToList() ?? new List<string>(),
                    Url = SanitizeString(project.Url, maximumProjectUrlLength)
                })
                .Where(project => !string.IsNullOrEmpty(project.Name))
                .ToList();
        }
        private List<ExtraCurricularActivity> ProcessActivities(List<ExtraCurricularActivity> activities)
        {
            if (activities == null || !activities.Any())
            {
                return new List<ExtraCurricularActivity>();
            }

            const int maximumNumberOfActivities = 10;
            const int maximumActivityNameLength = 150;
            const int maximumOrganizationNameLength = 100;
            const int maximumRoleNameLength = 80;
            const int maximumPeriodLength = 60;
            const int maximumActivityDescriptionLength = 300;

            return activities.Take(maximumNumberOfActivities)
                .Select(activity => new ExtraCurricularActivity
                {
                    ActivityName = SanitizeString(activity.ActivityName, maximumActivityNameLength),
                    Organization = SanitizeString(activity.Organization, maximumOrganizationNameLength),
                    Role = SanitizeString(activity.Role, maximumRoleNameLength),
                    Period = SanitizeString(activity.Period, maximumPeriodLength),
                    Description = SanitizeString(activity.Description, maximumActivityDescriptionLength)
                })
                .Where(activity => !string.IsNullOrEmpty(activity.ActivityName))
                .ToList();
        }

        /// <summary>
        /// Sanitizes and validates a string field
        /// </summary>
        private string SanitizeString(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            input = input.Trim();

            if (input.Length > maxLength)
            {
                input = input.Substring(0, maxLength);
            }

            return input;
        }

        /// <summary>
        /// Validates and sanitizes email
        /// </summary>
        private string SanitizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            email = email.Trim().ToLowerInvariant();

            if (email.Length > MaxEmailLength)
            {
                return string.Empty;
            }

            // Basic email validation
            if (!email.Contains('@') || !email.Contains('.'))
            {
                return string.Empty;
            }

            return email;
        }

        /// <summary>
        /// Validates age range
        /// </summary>
        private int ValidateAge(int age)
        {
            if (age < MinAge || age > MaxAge)
            {
                return InvalidAge;
            }
            return age;
        }

        /// <summary>
        /// Validates gender value
        /// </summary>
        private string ValidateGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
            {
                return string.Empty;
            }

            gender = gender.Trim();

            if (gender.Equals(GenderMale, StringComparison.OrdinalIgnoreCase) ||
        gender.Equals(GenderMaleShort, StringComparison.OrdinalIgnoreCase))
            {
                return GenderMale;
            }

            if (gender.Equals(GenderFemale, StringComparison.OrdinalIgnoreCase) ||
                gender.Equals(GenderFemaleShort, StringComparison.OrdinalIgnoreCase))
            {
                return GenderFemale;
            }

            return string.Empty;
        }

        /// <summary>
        /// Formats phone number with validation
        /// </summary>
        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }

            // Remove all non-digit characters except + at the beginning
            phoneNumber = phoneNumber.Trim();
            phoneNumber = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d+]", string.Empty);
            if (phoneNumber.Length > MaxPhoneLength)
            {
                return string.Empty;
            }

            return phoneNumber;
        }

        /// <summary>
        /// Validates graduation year
        /// </summary>
        private int ValidateGraduationYear(int year)
        {
            int currentYear = DateTime.Now.Year;

            if (year < currentYear || year > currentYear + MaxYearsAheadForGraduation)
            {
                return 0;
            }

            return year;
        }

        /// <summary>
        /// Validates date values
        /// </summary>
        private DateTimeOffset ValidateDate(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return DateTimeOffset.Now;
            }

            var minDate = new DateTimeOffset(MinValidDate);
            var maxDate = DateTimeOffset.Now.AddYears(MaxYearsAheadForDate);

            if (date < minDate || date > maxDate)
            {
                return DateTimeOffset.Now;
            }
            return date.Value;
        }
    }

    /// <summary>
    /// CV Data structure for deserialization
    /// </summary>
    public class CVData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string University { get; set; }
        public int ExpectedGraduationYear { get; set; }
        public string GitHub { get; set; }
        public string LinkedIn { get; set; }
        public string Address { get; set; }
        public string Motivation { get; set; }
        public bool HasDisabilities { get; set; }
        public List<string> Skills { get; set; }
        public List<WorkExperience> WorkExperiences { get; set; }
        public List<Project> Projects { get; set; }
        public List<ExtraCurricularActivity> ExtraCurricularActivities { get; set; }
    }
}