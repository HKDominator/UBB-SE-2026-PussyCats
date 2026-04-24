using System;
using Microsoft.UI.Xaml.Data;
using PussyCatsApp.Models;

namespace PussyCatsApp.Converters
{
    public class JobRoleToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not JobRole role)
            {
                if (value?.ToString() == null)
                {
                    return string.Empty;
                }
                return value?.ToString();
            }

            return role switch
            {
                JobRole.FrontendDeveloper => "Frontend Developer",
                JobRole.BackendDeveloper => "Backend Developer",
                JobRole.UIUXDesigner => "UI/UX Designer",
                JobRole.DevOpsEngineer => "DevOps Engineer",
                JobRole.ProjectManager => "Project Manager",
                JobRole.DataAnalyst => "Data Analyst",
                JobRole.CybersecuritySpecialist => "Cybersecurity Specialist",
                JobRole.AIMLEngineer => "AI/ML Engineer",
                var defaultRole => defaultRole.ToString()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
