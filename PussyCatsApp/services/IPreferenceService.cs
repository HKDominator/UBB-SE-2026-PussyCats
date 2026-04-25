using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Services
{
    public interface IPreferenceService
    {
        List<Preference> GetByUserId(int userId);
        void SavePreferences(int userId, List<JobRole> roles, WorkMode workMode, string location);
        List<string> SearchLocations(string query);
    }
}
