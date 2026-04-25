using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Utilities;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing the user profile form, including loading, editing, validating, and saving user profile data and handling CV parsing.
    /// </summary>
    public partial class ProfileFormViewModel : ObservableObject
    {
        private readonly IUserProfileService profileService;
        private readonly ICVParsingService cvParsingService;
        private UserProfile userProfile;

        // Required Fields
        [ObservableProperty] private string firstName = string.Empty;
        [ObservableProperty] private string lastName = string.Empty;
        [ObservableProperty] private double age;
        [ObservableProperty] private string gender = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string phonePrefix = string.Empty;
        [ObservableProperty] private string phoneNumber = string.Empty;
        [ObservableProperty] private string gitHub = string.Empty;
        [ObservableProperty] private string linkedIn = string.Empty;
        [ObservableProperty] private string country = string.Empty;
        [ObservableProperty] private string city = string.Empty;
        [ObservableProperty] private string university = string.Empty;
        [ObservableProperty] private string address = string.Empty;
        [ObservableProperty] private int expectedGraduationYear;
        [ObservableProperty] private string motivation = string.Empty;
        [ObservableProperty] private bool hasDisabilities;

        // Status
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private string cvStatusText = string.Empty;
        [ObservableProperty] private string infoBarMessage = string.Empty;
        [ObservableProperty] private InformationBarSeverityStatus infoBarSeverity; // 0=Informational, 1=Success, 2=Warning, 3=Error
        [ObservableProperty] private bool isInfoBarOpen;

        public ObservableCollection<string> Skills { get; } = new ();
        public ObservableCollection<WorkExperience> WorkExperiences { get; } = new ();
        public ObservableCollection<Project> Projects { get; } = new ();
        public ObservableCollection<ExtraCurricularActivity> ExtraCurricularActivities { get; } = new ();

        public List<int> GraduationYears { get; } = new ();

        private readonly int maximumNumberOfExtraCurricularActivitiesAllowed = 10;
        private readonly int maximumNumberOfSkillsAllowed = 30;
        private readonly int maximumSkillNameLength = 60;
        private readonly int maximumNumberOfWorkExperiencesAllowed = 10;
        private readonly int maximumNumberOfProjectsAllowed = 10;
        private readonly int missingAgeDefaultValue = 0;
        private readonly int missingGraduationYearDefaultValue = 0;

        public ProfileFormViewModel(IUserProfileService profileService, ICVParsingService cvParsingService)
        {
            this.profileService = profileService;
            this.cvParsingService = cvParsingService;

            int currentYear = DateTime.Now.Year;
            int numberOfYearsToShow = 10;
            for (int year = currentYear; year <= currentYear + numberOfYearsToShow; year++)
            {
                GraduationYears.Add(year);
            }
        }

        public static ProfileFormViewModel Create()
        {
            var userProfileRepo = new UserProfileRepository(DatabaseConfiguration.GetConnectionString());
            var skillTestRepo = new SkillTestRepository(DatabaseConfiguration.GetConnectionString());
            var profileService = new UserProfileService(skillTestRepo, userProfileRepo);
            var cvParsingService = new CVParsingService();
            return new ProfileFormViewModel(profileService, cvParsingService);
        }

        public void LoadProfile(UserProfile profile)
        {
            if (profile != null)
            {
                userProfile = profile;
            }
            else
            {
                userProfile = new UserProfile();
            }

            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            Age = userProfile.Age;
            Gender = userProfile.Gender;
            Email = userProfile.Email;
            GitHub = userProfile.GitHub;
            LinkedIn = userProfile.LinkedIn;
            University = userProfile.University;
            Address = userProfile.Address;
            Motivation = userProfile.Motivation;
            Country = userProfile.Country;
            City = userProfile.City;
            ExpectedGraduationYear = userProfile.ExpectedGraduationYear;
            HasDisabilities = userProfile.HasDisabilities;

            // Extract phone prefix and number
            if (!string.IsNullOrEmpty(userProfile.PhoneNumber))
            {
                var phoneNumberParts = PhoneNumberHelper.ExtractPhonePrefixAndNumber(userProfile.PhoneNumber);
                PhonePrefix = phoneNumberParts.prefix;
                PhoneNumber = phoneNumberParts.number;
            }

            Skills.Clear();
            foreach (var skill in userProfile.Skills)
            {
                Skills.Add(skill);
            }

            WorkExperiences.Clear();
            foreach (var workExperience in userProfile.WorkExperiences)
            {
                WorkExperiences.Add(workExperience);
            }

            Projects.Clear();
            foreach (var project in userProfile.Projects)
            {
                Projects.Add(project);
            }

            ExtraCurricularActivities.Clear();
            foreach (var extraCurricularActivity in userProfile.ExtraCurricularActivities)
            {
                ExtraCurricularActivities.Add(extraCurricularActivity);
            }
        }

        public void AddSkill(string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
            {
                return;
            }

            skill = skill.Trim();

            if (IsDuplicateSkill(skill))
            {
                ShowInfoBar("This skill has already been added.", InformationBarSeverityStatus.Warning);
                return;
            }

            if (Skills.Count >= maximumNumberOfSkillsAllowed)
            {
                ShowInfoBar($"Maximum of {maximumNumberOfSkillsAllowed} skills allowed.", InformationBarSeverityStatus.Warning);
                return;
            }

            if (skill.Length > maximumSkillNameLength)
            {
                ShowInfoBar($"Skill name must be less than {maximumSkillNameLength} characters.", InformationBarSeverityStatus.Warning);
                return;
            }

            Skills.Add(skill);
        }

        public void RemoveSkill(string skill)
        {
            Skills.Remove(skill);
        }

        public void AddWorkExperience()
        {
            if (WorkExperiences.Count >= maximumNumberOfWorkExperiencesAllowed)
            {
                ShowInfoBar($"Maximum of {maximumNumberOfWorkExperiencesAllowed} work experiences allowed.", InformationBarSeverityStatus.Warning);
                return;
            }

            WorkExperiences.Add(new WorkExperience
            {
                StartDate = DateTimeOffset.Now,
                CurrentlyWorking = false
            });
        }

        public void RemoveWorkExperience(WorkExperience workExperience)
        {
            WorkExperiences.Remove(workExperience);
        }

        public void AddProject()
        {
            if (Projects.Count >= maximumNumberOfProjectsAllowed)
            {
                ShowInfoBar($"Maximum of {maximumNumberOfProjectsAllowed} projects allowed.", InformationBarSeverityStatus.Warning);
                return;
            }

            Projects.Add(new Project());
        }

        public void RemoveProject(Project project)
        {
            Projects.Remove(project);
        }

        public void AddExtraCurricularActivity()
        {
            if (ExtraCurricularActivities.Count >= maximumNumberOfExtraCurricularActivitiesAllowed)
            {
                ShowInfoBar($"Maximum of {maximumNumberOfExtraCurricularActivitiesAllowed} extra-curricular activities allowed.", InformationBarSeverityStatus.Warning);
                return;
            }

            ExtraCurricularActivities.Add(new ExtraCurricularActivity());
        }

        public void RemoveExtraCurricularActivity(ExtraCurricularActivity activity)
        {
            ExtraCurricularActivities.Remove(activity);
        }

        public bool SaveProfile()
        {
            var errors = ProfileFormValidator.ValidateForm(FirstName, LastName, Age, Gender, Email, PhonePrefix, PhoneNumber, Country, City, University, ExpectedGraduationYear, WorkExperiences.ToList());
            if (errors.Any())
            {
                ShowInfoBar($"Please fill in required fields: {string.Join(", ", errors)}", InformationBarSeverityStatus.Error);
                return false;
            }

            UpdateProfileFromForm();

            try
            {
                profileService.SaveProfile(userProfile.UserId, userProfile);
                ShowInfoBar("Profile saved successfully!", InformationBarSeverityStatus.Success);
                return true;
            }
            catch (Exception exception)
            {
                ShowInfoBar($"Error saving profile: {exception.Message}", InformationBarSeverityStatus.Error);
                return false;
            }
        }

        public UserProfile GetUpdatedProfile()
        {
            UpdateProfileFromForm();
            return userProfile;
        }

        private void UpdateProfileFromForm()
        {
            userProfile.FirstName = FirstName.Trim();
            userProfile.LastName = LastName.Trim();
            userProfile.Age = (int)Age;
            userProfile.Gender = Gender;
            userProfile.Email = Email.Trim().ToLowerInvariant();
            userProfile.PhoneNumber = PhonePrefix + PhoneNumber.Trim();
            userProfile.GitHub = GitHub.Trim();
            userProfile.LinkedIn = LinkedIn.Trim();
            userProfile.Country = Country;
            userProfile.City = City.Trim();
            userProfile.University = University.Trim();
            userProfile.ExpectedGraduationYear = ExpectedGraduationYear;
            userProfile.Address = Address.Trim();
            userProfile.Motivation = Motivation.Trim();
            userProfile.HasDisabilities = HasDisabilities;
            userProfile.Skills = Skills.ToList();
            userProfile.WorkExperiences = WorkExperiences.ToList();
            userProfile.Projects = Projects.ToList();
            userProfile.ExtraCurricularActivities = ExtraCurricularActivities.ToList();
            userProfile.LastUpdated = DateTime.Now;
        }

        public void ProcessCVFile(string content, string fileType)
        {
            try
            {
                var userProfileParsedFromCvFile = cvParsingService.ParseCVFile(content, fileType);
                PopulateFromParsedProfile(userProfileParsedFromCvFile);
                CvStatusText = "CV loaded successfully!";
                ShowInfoBar("CV data has been loaded. Please review and complete any missing fields.", InformationBarSeverityStatus.Success);
            }
            catch (Exception exception)
            {
                var errorMessage = exception.InnerException?.Message;
                if (errorMessage == null)
                {
                    errorMessage = exception.Message;
                }
                ShowInfoBar($"Error processing CV file: {errorMessage}", InformationBarSeverityStatus.Error);
            }
        }

        public void PopulateFromParsedProfile(UserProfile parsedUserProfile)
        {
            if (!string.IsNullOrEmpty(parsedUserProfile.FirstName))
            {
                FirstName = parsedUserProfile.FirstName;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.LastName))
            {
                LastName = parsedUserProfile.LastName;
            }

            if (parsedUserProfile.Age > missingAgeDefaultValue)
            {
                Age = parsedUserProfile.Age;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.Gender))
            {
                Gender = parsedUserProfile.Gender;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.Email))
            {
                Email = parsedUserProfile.Email;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.PhoneNumber))
            {
                var parts = PhoneNumberHelper.ExtractPhonePrefixAndNumber(parsedUserProfile.PhoneNumber);
                PhonePrefix = parts.prefix;
                PhoneNumber = parts.number;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.GitHub))
            {
                GitHub = parsedUserProfile.GitHub;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.LinkedIn))
            {
                LinkedIn = parsedUserProfile.LinkedIn;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.Country))
            {
                Country = parsedUserProfile.Country;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.City))
            {
                City = parsedUserProfile.City;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.University))
            {
                University = parsedUserProfile.University;
            }

            if (parsedUserProfile.ExpectedGraduationYear > missingGraduationYearDefaultValue)
            {
                ExpectedGraduationYear = parsedUserProfile.ExpectedGraduationYear;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.Address))
            {
                Address = parsedUserProfile.Address;
            }

            if (!string.IsNullOrEmpty(parsedUserProfile.Motivation))
            {
                Motivation = parsedUserProfile.Motivation;
            }

            // Clear existing collections before loading new CV data
            Skills.Clear();
            WorkExperiences.Clear();
            Projects.Clear();
            ExtraCurricularActivities.Clear();

            if (parsedUserProfile.Skills != null)
            {
                foreach (var skill in parsedUserProfile.Skills)
                {
                    if (!IsDuplicateSkill(skill) && Skills.Count < maximumNumberOfSkillsAllowed)
                    {
                        Skills.Add(skill);
                    }
                }
            }

            if (parsedUserProfile.WorkExperiences != null)
            {
                foreach (var workExperience in parsedUserProfile.WorkExperiences)
                {
                    if (WorkExperiences.Count < maximumNumberOfWorkExperiencesAllowed)
                    {
                        WorkExperiences.Add(workExperience);
                    }
                }
            }

            if (parsedUserProfile.Projects != null)
            {
                foreach (var project in parsedUserProfile.Projects)
                {
                    if (Projects.Count < maximumNumberOfProjectsAllowed)
                    {
                        Projects.Add(project);
                    }
                }
            }

            if (parsedUserProfile.ExtraCurricularActivities != null)
            {
                foreach (var extraCurricularActivity in parsedUserProfile.ExtraCurricularActivities)
                {
                    if (ExtraCurricularActivities.Count < maximumNumberOfExtraCurricularActivitiesAllowed)
                    {
                        ExtraCurricularActivities.Add(extraCurricularActivity);
                    }
                }
            }

            var missingFields = new List<string>();
            var fieldsOfTypeString = new Dictionary<string, string>
            {
                { "First Name", FirstName },
                { "Last Name", LastName },
                { "Gender", Gender },
                { "Email", Email },
                { "Phone Number", PhoneNumber },
                { "Country", Country },
                { "City", City },
                { "University", University }
            };

            foreach (var field in fieldsOfTypeString)
            {
                if (string.IsNullOrEmpty(field.Value))
                {
                    missingFields.Add(field.Key);
                }
            }

            if (Age == missingAgeDefaultValue)
            {
                missingFields.Add("Age");
            }

            if (ExpectedGraduationYear == missingGraduationYearDefaultValue)
            {
                missingFields.Add("Expected Graduation Year");
            }

            if (missingFields.Any())
            {
                ShowInfoBar($"Missing fields: {string.Join(", ", missingFields)}", InformationBarSeverityStatus.Warning);
            }
        }

        public List<string> FilterUniversities(string query)
        {
            return ProfileFormHelpers.FilterUniversitiesHelper(query);
        }

        public List<string> FilterSkillSuggestions(string searchTextQuery)
        {
            if (string.IsNullOrWhiteSpace(searchTextQuery))
            {
                return new List<string>();
            }

            var searchText = searchTextQuery.ToLower();
            var results = new List<string>();
            foreach (string skill in ProfileFormData.SkillSuggestions)
            {
                if (SkillMatchesSearchAndIsNotDuplicate(skill, searchText))
                {
                    results.Add(skill);
                }
            }
            return results;
        }

        private void ShowInfoBar(string message, InformationBarSeverityStatus severity)
        {
            InfoBarMessage = message;
            InfoBarSeverity = severity;
            IsInfoBarOpen = true;
        }

        public bool IsDuplicateSkill(string skill)
        {
            foreach (string existingSkill in Skills)
            {
                if (existingSkill.Equals(skill, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        public bool SkillMatchesSearchAndIsNotDuplicate(string skill, string searchText)
        {
            return skill.ToLower().Contains(searchText) && !IsDuplicateSkill(skill);
        }
    }
}
