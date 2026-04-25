using System;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services;

public static class UserLevelService
{
    public static int GetLevelProgressPercent(int totalXp, UserLevel userLevel)
    {
        if (totalXp < 0)
        {
            throw new ArgumentException("XP cannot be negative.");
        }

        if (userLevel.NextLevelXp == 0)
        {
            return 100;
        }

        double completedPercentageIntoCurrentLevel = GetLevelProgressPercentage(totalXp, userLevel);
        return (int)completedPercentageIntoCurrentLevel;
    }

    private static double GetLevelProgressPercentage(int totalXp, UserLevel userLevel)
    {
        double pointsIntoLevel = totalXp - userLevel.XpRequired;
        double totalPointsForLevel = userLevel.NextLevelXp - userLevel.XpRequired;
        double completedPercentageIntoCurrentLevel = pointsIntoLevel / totalPointsForLevel * 100;
        return completedPercentageIntoCurrentLevel;
    }

    public static int GetXpToNextLevel(int totalXp, UserLevel userLevel)
    {
        if (totalXp < 0)
        {
            throw new ArgumentException("XP cannot be negative.");
        }

        if (userLevel.NextLevelXp == UserLevel.LEVEL_1_XP)
        {
            return 0;
        }
        return userLevel.NextLevelXp - totalXp;
    }

    public static UserLevel CalculateLevel(int experiencePoints)
    {
        if (experiencePoints < 0)
        {
            throw new ArgumentException("XP cannot be negative.");
        }
        switch (experiencePoints)
        {
            case >= UserLevel.LEVEL_5_XP:
                return new UserLevel(5, UserLevel.UserTitle.Expert, UserLevel.LEVEL_5_XP, UserLevel.LEVEL_1_XP);
            case >= UserLevel.LEVEL_4_XP:
                return new UserLevel(4, UserLevel.UserTitle.Specialist, UserLevel.LEVEL_4_XP, UserLevel.LEVEL_5_XP);
            case >= UserLevel.LEVEL_3_XP:
                return new UserLevel(3, UserLevel.UserTitle.Practitioner, UserLevel.LEVEL_3_XP, UserLevel.LEVEL_4_XP);
            case >= UserLevel.LEVEL_2_XP:
                return new UserLevel(2, UserLevel.UserTitle.Apprentice, UserLevel.LEVEL_2_XP, UserLevel.LEVEL_3_XP);
            default:
                return new UserLevel(1, UserLevel.UserTitle.Newcomer, UserLevel.LEVEL_1_XP, UserLevel.LEVEL_2_XP);
        }
    }
}