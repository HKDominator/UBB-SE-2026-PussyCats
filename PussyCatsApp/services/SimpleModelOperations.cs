using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Services;

public static class SimpleModelOperations
{
    private const float GoldThreshold = 90f;
    private const float SilverThreshold = 70f;
    private const float BronzeThreshold = 50f;
    private const int GoldExperiencePoints = 100;
    private const int SilverExperiencePoints = 60;
    private const int BronzeExperiencePoints = 30;
    private const int ParticipantExperiencePoints = 10;

    public static Badge AssignTier(float score)
    {
        switch (score)
        {
            case >= GoldThreshold:
                return new Badge(BadgeTier.GOLD, "ms-appx:///Assets/badges/gold.svg", GoldExperiencePoints);
            case >= SilverThreshold:
                return new Badge(BadgeTier.SILVER, "ms-appx:///Assets/badges/silver.svg", SilverExperiencePoints);
            case >= BronzeThreshold:
                return new Badge(BadgeTier.BRONZE, "ms-appx:///Assets/badges/bronze.svg", BronzeExperiencePoints);
            default:
                return new Badge(BadgeTier.PARTICIPANT, "ms-appx:///Assets/badges/participant.svg", ParticipantExperiencePoints);
        }
    }
}