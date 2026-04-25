using System;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services;

public static class UserLevelService
{
    private const int FullProgressPercentage = 100;
    private const int NoExperiencePointsRemaining = 0;
    public static int GetLevelProgressPercent(int totalExperiencePoints, UserLevel userLevel)
    {
        if (totalExperiencePoints < 0)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }

        if (userLevel.NextLevelExperiencePoints == 0)
        {
            return FullProgressPercentage;
        }

        double completedPercentageIntoCurrentLevel = GetLevelProgressPercentage(totalExperiencePoints, userLevel);
        return (int)completedPercentageIntoCurrentLevel;
    }

    private static double GetLevelProgressPercentage(int totalExperiencePoints, UserLevel userLevel)
    {
        double pointsIntoLevel = totalExperiencePoints - userLevel.ExperiencePointsRequired;
        double totalPointsForLevel = userLevel.NextLevelExperiencePoints - userLevel.ExperiencePointsRequired;
        double completedPercentageIntoCurrentLevel = pointsIntoLevel / totalPointsForLevel * FullProgressPercentage;
        return completedPercentageIntoCurrentLevel;
    }

    public static int GetExperiencePointsToNextLevel(int totalExperiencePoints, UserLevel userLevel)
    {
        if (totalExperiencePoints < 0)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }

        if (userLevel.NextLevelExperiencePoints == UserLevel.LEVEL_1_ExperiencePoints)
        {
            return NoExperiencePointsRemaining;
        }
        return userLevel.NextLevelExperiencePoints - totalExperiencePoints;
    }

    public static UserLevel CalculateLevel(int experiencePoints)
    {
        if (experiencePoints < 0)
        {
            throw new ArgumentException("Experience Points cannot be negative.");
        }
        switch (experiencePoints)
        {
            case >= UserLevel.LEVEL_5_ExperiencePoints:
                return new UserLevel(5, UserLevel.UserTitle.Expert, UserLevel.LEVEL_5_ExperiencePoints, UserLevel.LEVEL_1_ExperiencePoints);
            case >= UserLevel.LEVEL_4_ExperiencePoints:
                return new UserLevel(4, UserLevel.UserTitle.Specialist, UserLevel.LEVEL_4_ExperiencePoints, UserLevel.LEVEL_5_ExperiencePoints);
            case >= UserLevel.LEVEL_3_ExperiencePoints:
                return new UserLevel(3, UserLevel.UserTitle.Practitioner, UserLevel.LEVEL_3_ExperiencePoints, UserLevel.LEVEL_4_ExperiencePoints);
            case >= UserLevel.LEVEL_2_ExperiencePoints:
                return new UserLevel(2, UserLevel.UserTitle.Apprentice, UserLevel.LEVEL_2_ExperiencePoints, UserLevel.LEVEL_3_ExperiencePoints);
            default:
                return new UserLevel(1, UserLevel.UserTitle.Newcomer, UserLevel.LEVEL_1_ExperiencePoints, UserLevel.LEVEL_2_ExperiencePoints);
        }
    }
}