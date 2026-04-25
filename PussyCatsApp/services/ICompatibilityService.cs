using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Services
{
    public interface ICompatibilityService
    {
        RoleResult CalculateForRole(int userId, JobRole role);

        List<RoleResult> CalculateAll(int userId);

        List<Suggestion> GetSuggestions(RoleResult result);
    }
}