using System;
using System.Collections.Generic;
using System.Text;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ISkillTestRepository skillTestRepository;
        private readonly IUserProfileRepository userProfileRepository;

        public UserProfileService(ISkillTestRepository skillTestRepository, IUserProfileRepository userProfileRepository)
        {
            this.skillTestRepository = skillTestRepository;
            this.userProfileRepository = userProfileRepository;
        }

        public UserProfile GetProfile(int userId)
        {
            return userProfileRepository.GetProfileById(userId);
        }

        public List<SkillTest> GetSkillTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool IsProfileAvailable(int userId)
        {
            UserProfile userProfile = userProfileRepository.GetProfileById(userId);

            if (userProfile == null)
            {
                throw new Exception($"No profile found for ID {userId}");
            }

            return userProfile.ActiveAccount;
        }

        public void ToggleAccountStatus(int userId, string currentAccountStatus)
        {
            string newAccountStatus = currentAccountStatus == AccountStatus.Active.ToString().ToUpper()
                ? AccountStatus.Inactive.ToString().ToUpper()
                : AccountStatus.Active.ToString().ToUpper();

            userProfileRepository.UpdateAccountStatus(userId, newAccountStatus);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public void UpdateProfilePicturePath(int userId, string newProfilePicturePath)
        {
            userProfileRepository.UpdateProfilePicture(userId, newProfilePicturePath);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public void RemoveProfilePicturePath(int userId)
        {
            userProfileRepository.UpdateProfilePicture(userId, null);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public string GenerateParsedCVText(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return string.Empty;
            }

            var parsedCvTextBuilder = new StringBuilder();
            parsedCvTextBuilder.AppendLine($"{userProfile.FirstName} {userProfile.LastName}".Trim());
            parsedCvTextBuilder.AppendLine(userProfile.University ?? string.Empty);
            parsedCvTextBuilder.AppendLine(string.Join(", ", userProfile.Skills ?? new List<string>()));
            return parsedCvTextBuilder.ToString().TrimEnd();
        }
        public void SaveProfile(int userId, UserProfile userProfile)
        {
            userProfile.ParsedCV = GenerateParsedCVText(userProfile);
            userProfileRepository.Save(userId, userProfile);
            userProfileRepository.UpdateProfileLastModified(userId, DateTime.Now);
        }

        public int RecalculateLevel(UserProfile userProfile)
        {
            if (userProfile == null)
            {
                return 0;
            }

            int totalExperiencePoints = 0;

            List<SkillTest> skillTests = GetSkillTestsForUser(userProfile.UserId);
            foreach (SkillTest skillTest in skillTests)
            {
                totalExperiencePoints += SkillTestService.GetExperiencePoints(skillTest);
            }

            userProfile.UserLevel = UserLevelService.CalculateLevel(totalExperiencePoints);

            return totalExperiencePoints;
        }
    }
}