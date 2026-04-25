using System;
using System.Collections.Generic;

namespace PussyCatsApp.Models
{
    public class UserProfile
    {
        // Required Fields from R16
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty; // Male, Female
        public string Country { get; set; } = string.Empty;

        // The city field is not a part of our app
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int UniversityStartYear { get; set; }
        public int ExpectedGraduationYear { get; set; }

        // Optional Fields from R16
        public List<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<ExtraCurricularActivity> ExtraCurricularActivities { get; set; } = new List<ExtraCurricularActivity>();
        public List<string> Skills { get; set; } = new List<string>();
        public string Motivation { get; set; } = string.Empty;
        public bool HasDisabilities { get; set; }
        public List<string> RelevantCertificates { get; set; } = new List<string>();

        // Additional profile fields from requirements
        public string GitHub { get; set; } = string.Empty;
        public string LinkedIn { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool ActiveAccount { get; set; } = true;
        public string ProfilePicture { get; set; } = string.Empty;
        public string PersonalityTestResult { get; set; } = string.Empty;
        public string ParsedCV { get; set; } = string.Empty;
        public string FormDataJson { get; set; } = string.Empty;

        // Job preferences (R13-R15)
        public List<string> PreferredJobRoles { get; set; } = new List<string>();
        public string WorkModePreference { get; set; } = string.Empty; // Remote, Hybrid, On-site
        public string LocationPreference { get; set; } = string.Empty;

        // Timestamps
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public UserLevel UserLevel { get; set; } = new UserLevel();
        public int TotalXP { get; set; } = 0;
    }

    public class WorkExperience
    {
        public string Company { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; } // null if currently working
        public string Description { get; set; } = string.Empty;
        public bool CurrentlyWorking { get; set; }
    }

    public class Project
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = new List<string>();
        public string Url { get; set; } = string.Empty;
    }

    public class ExtraCurricularActivity
    {
        public string ActivityName { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}