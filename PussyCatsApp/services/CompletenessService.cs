using System;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class CompletenessService : ICompletenessService
    {
        private const int TotalFields = 21;

        private static readonly string[] Labels =
        {
            "First Name", "Last Name", "Age", "Gender", "Country",
            "Phone Number", "Email", "University", "Graduation Year", "GitHub",
            "LinkedIn", "Address", "Profile Picture", "Skills", "Motivation",
            "Work Experience", "Projects", "Activities", "Preferred Roles",
            "Work Mode", "Location Preference"
        };
        private bool IsFieldFilled(int index, UserProfile userProfile)
        {
            switch (index)
            {
                case 0: return !string.IsNullOrWhiteSpace(userProfile.FirstName);
                case 1: return !string.IsNullOrWhiteSpace(userProfile.LastName);
                case 2: return userProfile.Age > 0;
                case 3: return !string.IsNullOrWhiteSpace(userProfile.Gender);
                case 4: return !string.IsNullOrWhiteSpace(userProfile.Country);
                case 5: return !string.IsNullOrWhiteSpace(userProfile.PhoneNumber);
                case 6: return !string.IsNullOrWhiteSpace(userProfile.Email);
                case 7: return !string.IsNullOrWhiteSpace(userProfile.University);
                case 8: return userProfile.ExpectedGraduationYear > 0;
                case 9: return !string.IsNullOrWhiteSpace(userProfile.GitHub);
                case 10: return !string.IsNullOrWhiteSpace(userProfile.LinkedIn);
                case 11: return !string.IsNullOrWhiteSpace(userProfile.Address);
                case 12: return !string.IsNullOrWhiteSpace(userProfile.ProfilePicture);
                case 13: return userProfile.Skills != null && userProfile.Skills.Count > 0;
                case 14: return !string.IsNullOrWhiteSpace(userProfile.Motivation);
                case 15: return userProfile.WorkExperiences != null && userProfile.WorkExperiences.Count > 0;
                case 16: return userProfile.Projects != null && userProfile.Projects.Count > 0;
                case 17: return userProfile.ExtraCurricularActivities != null && userProfile.ExtraCurricularActivities.Count > 0;
                case 18: return userProfile.PreferredJobRoles != null && userProfile.PreferredJobRoles.Count > 0;
                case 19: return !string.IsNullOrWhiteSpace(userProfile.WorkModePreference);
                case 20: return !string.IsNullOrWhiteSpace(userProfile.LocationPreference);
                default: return false;
            }
        }
        private int CalculatePercentage(int filledFields)
        {
            return (int)Math.Round((double)filledFields / TotalFields * 100);
        }
        private int CountFilledFields(UserProfile profile)
        {
            int filledFields = 0;

            for (int i = 0; i < TotalFields; i++)
            {
                if (IsFieldFilled(i, profile))
                {
                    filledFields++;
                }
            }

            return filledFields;
        }

        public int CalculateCompleteness(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return 0;
            }
            int filledFields = CountFilledFields(userProfile);
            return CalculatePercentage(filledFields);
        }

        public string GetNextEmptyFieldPrompt(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return string.Empty;
            }

            int filledFields = CountFilledFields(userProfile);

            for (int fieldIndex = 0; fieldIndex < TotalFields; fieldIndex++)
            {
                if (!IsFieldFilled(fieldIndex, userProfile))
                {
                    int nextPercentage = CalculatePercentage(filledFields + 1);
                    return $"Add your {Labels[fieldIndex]} to reach {nextPercentage}% completeness!";
                }
            }

            return "Your profile is 100% complete!";
        }
    }
}