using System;
using System.Collections.Generic;
using PussyCatsApp.Configuration;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    public class PreferencesViewModel
    {
        private List<JobRole> selectedJobRoles;
        private WorkMode selectedWorkMode;
        private string preferredLocation;
        private List<string> locationSuggestions;
        private string errorMessage;
        private int currentUserId;
        private IPreferenceService preferencesService;

        private const int MaximumJobRolesAllowed = 3;

        public PreferencesViewModel(IPreferenceService preferenceService, int userId)
        {
            this.preferencesService = preferenceService;
            currentUserId = userId;
            selectedJobRoles = new List<JobRole>();
            locationSuggestions = new List<string>();
            preferredLocation = string.Empty;
            errorMessage = string.Empty;
        }

        public void LoadPreferences()
        {
            selectedJobRoles.Clear();
            errorMessage = string.Empty;

            var savedPreferences = preferencesService.GetByUserId(currentUserId);

            foreach (var preference in savedPreferences)
            {
                if (preference.PreferenceType == "JobRole")
                {
                    if (Enum.TryParse<JobRole>(preference.Value, out var jobRole))
                    {
                        selectedJobRoles.Add(jobRole);
                    }
                }
                else if (preference.PreferenceType == "WorkMode")
                {
                    if (Enum.TryParse<WorkMode>(preference.Value, out var workMode))
                    {
                        selectedWorkMode = workMode;
                    }
                }
                else if (preference.PreferenceType == "Location")
                {
                    preferredLocation = preference.Value;
                }
            }
        }

        public void ToggleJobRole(JobRole jobRole)
        {
            errorMessage = string.Empty;

            if (selectedJobRoles.Contains(jobRole))
            {
                selectedJobRoles.Remove(jobRole);
            }
            else
            {
                if (selectedJobRoles.Count < MaximumJobRolesAllowed)
                {
                    selectedJobRoles.Add(jobRole);
                }
                else
                {
                    errorMessage = $"You can select a maximum of {MaximumJobRolesAllowed} job roles.";
                }
            }
        }

        public void SetWorkMode(WorkMode workMode)
        {
            selectedWorkMode = workMode;
        }

        public void SetLocation(string location)
        {
            preferredLocation = location;
        }

        public void SearchLocation(string query)
        {
            locationSuggestions = preferencesService.SearchLocations(query);
        }

        public void SavePreferences()
        {
            errorMessage = string.Empty;
            try
            {
                preferencesService.SavePreferences(currentUserId, selectedJobRoles, selectedWorkMode, preferredLocation);
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        public List<JobRole> GetSelectedJobRoles()
        {
            return selectedJobRoles;
        }

        public WorkMode GetSelectedWorkMode()
        {
            return selectedWorkMode;
        }

        public string GetPreferredLocation()
        {
            return preferredLocation;
        }

        public List<string> GetLocationSuggestions()
        {
            return locationSuggestions;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}