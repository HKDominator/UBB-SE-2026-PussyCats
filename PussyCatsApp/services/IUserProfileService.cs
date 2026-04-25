using System.Collections.Generic;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public interface IUserProfileService
    {
        UserProfile GetProfile(int userId);

        List<SkillTest> GetSkillTestsForUser(int userId);

        bool IsProfileAvailable(int userId);

        void ToggleAccountStatus(int userId, string currentStatus);

        void UpdateProfilePicturePath(int userId, string newPath);

        void RemoveProfilePicturePath(int userId);

        string GenerateParsedCVText(UserProfile profile);

        void SaveProfile(int userId, UserProfile profile);

        int RecalculateLevel(UserProfile profile);
    }
}