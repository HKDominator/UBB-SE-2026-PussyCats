namespace PussyCatsApp.Models
{
    public class UserLevel
    {
        public int LevelNumber { get; set; }
        public UserTitle Title { get; set; }
        public int XpRequired { get; set; }
        public int NextLevelXp { get; set; }

        public UserLevel(int levelNumber, UserLevel.UserTitle title, int xpRequired, int nextLevelXp)
        {
            LevelNumber = levelNumber;
            Title = title;
            XpRequired = xpRequired;
            NextLevelXp = nextLevelXp;
        }
        public UserLevel()
        {
            LevelNumber = 1;
            Title = UserLevel.UserTitle.Newcomer;
            XpRequired = LEVEL_1_XP;
            NextLevelXp = LEVEL_2_XP;
        }

        public enum UserTitle
        {
            Newcomer,
            Apprentice,
            Practitioner,
            Specialist,
            Expert
        }

        public const int LEVEL_1_XP = 0;
        public const int LEVEL_2_XP = 100;
        public const int LEVEL_3_XP = 250;
        public const int LEVEL_4_XP = 500;
        public const int LEVEL_5_XP = 800;
    }
}
