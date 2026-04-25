namespace PussyCatsApp.Models
{
    public class UserLevel
    {
        public int LevelNumber { get; set; }
        public UserTitle Title { get; set; }
        public int ExperiencePointsRequired { get; set; }
        public int NextLevelExperiencePoints { get; set; }

        public UserLevel(int levelNumber, UserLevel.UserTitle title, int experiencePointsRequired, int nextLevelExperiencePoints)
        {
            LevelNumber = levelNumber;
            Title = title;
            ExperiencePointsRequired = experiencePointsRequired;
            NextLevelExperiencePoints = nextLevelExperiencePoints;
        }
        public UserLevel()
        {
            LevelNumber = 1;
            Title = UserLevel.UserTitle.Newcomer;
            ExperiencePointsRequired = LEVEL_1_ExperiencePoints;
            NextLevelExperiencePoints = LEVEL_2_ExperiencePoints;
        }

        public enum UserTitle
        {
            Newcomer,
            Apprentice,
            Practitioner,
            Specialist,
            Expert
        }

        public const int LEVEL_1_ExperiencePoints = 0;
        public const int LEVEL_2_ExperiencePoints = 100;
        public const int LEVEL_3_ExperiencePoints = 250;
        public const int LEVEL_4_ExperiencePoints = 500;
        public const int LEVEL_5_ExperiencePoints = 800;
    }
}
