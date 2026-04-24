using System;
using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.Utilities
{
    public class ProfileFormValidator
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> ValidateForm(string firstName, string lastName, double age, string gender, string email, string phonePrefix, string phoneNumber, string country, string city, string university, int expectedGraduationYear, List<WorkExperience> workExperiences)
        {
            int minimumAge = 16, maximumAge = 60;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add("First Name");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add("Last Name");
            }

            if (age < minimumAge || age > maximumAge)
            {
                errors.Add($"Age (must be between {minimumAge}-{maximumAge})");
            }

            if (string.IsNullOrWhiteSpace(gender))
            {
                errors.Add("Gender");
            }

            if (string.IsNullOrWhiteSpace(email) || !ProfileFormValidator.IsValidEmail(email))
            {
                errors.Add("Valid Email");
            }

            if (string.IsNullOrWhiteSpace(phonePrefix) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                errors.Add("Phone Number");
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                errors.Add("Country");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                errors.Add("City");
            }

            if (string.IsNullOrWhiteSpace(university))
            {
                errors.Add("University");
            }
            if (expectedGraduationYear == 0)
            {
                errors.Add("Expected Graduation Year");
            }

            foreach (var we in workExperiences)
            {
                if (!we.CurrentlyWorking && we.EndDate.HasValue && we.EndDate.Value < we.StartDate)
                {
                    errors.Add($"Work Experience \"{we.Company}\": End date is before start date");
                }
            }

            return errors;
        }
    }
}
