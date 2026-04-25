using System;
using System.Collections.Generic;
using PussyCatsApp.Repositories;
using PussyCatsApp.Models;

namespace PussyCatsApp.Services
{
    public class SkillTestService : ISkillTestService
    {
        /// <summary>
        /// Score threshold at or above which the gold tier is awarded.
        /// </summary>
        private const int GoldScoreThreshold = 90;

        /// <summary>
        /// Score threshold at or above which the silver tier is awarded.
        /// </summary>
        private const int SilverScoreThreshold = 70;

        /// <summary>
        /// Score threshold at or above which the bronze tier is awarded.
        /// </summary>
        private const int BronzeScoreThreshold = 50;

        /// <summary>
        /// Experience points awarded for a gold-tier score.
        /// </summary>
        private const int GoldExperiencePoints = 100;

        /// <summary>
        /// Experience points awarded for a silver-tier score.
        /// </summary>
        private const int SilverExperiencePoints = 60;

        /// <summary>
        /// Experience points awarded for a bronze-tier score.
        /// </summary>
        private const int BronzeExperiencePoints = 30;

        /// <summary>
        /// Number of months that must pass before a test can be retaken.
        /// </summary>
        private const int RetakeEligibilityMonths = 3;

        /// <summary>
        /// Experience points awarded for a participant-tier score.
        /// </summary>
        private const int ParticipantExperiencePoints = 10;

        private ISkillTestRepository skillTestRepository;

        public SkillTestService(ISkillTestRepository skillTestRepository)
        {
            this.skillTestRepository = skillTestRepository;
        }

        public List<SkillTest> GetTestsForUser(int userId)
        {
            return skillTestRepository.GetSkillTestsByUserId(userId);
        }

        public bool CanRetakeTest(int skillId)
        {
            SkillTest skillTest = skillTestRepository.Load(skillId);

            if (skillTest == null)
            {
                throw new Exception($"No test found for ID {skillId}");
            }

            return IsRetakeEligible(skillTest);
        }

        public static bool IsRetakeEligible(SkillTest skillTest)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly eligibilityDate = currentDate.AddMonths(-RetakeEligibilityMonths);

            return eligibilityDate >= skillTest.AchievedDate;
        }

        public Badge SubmitRetake(int skillId, int newScore)
        {
            if (!CanRetakeTest(skillId))
            {
                throw new Exception("Test is not yet eligible for a retake. Action blocked at service layer.");
            }

            skillTestRepository.UpdateSkillTestScore(skillId, newScore);
            skillTestRepository.UpdateAchievedDate(skillId, DateOnly.FromDateTime(DateTime.Now));

            return SimpleModelOperations.AssignTier(newScore);
        }

        public static int GetExperiencePoints(SkillTest testSkill)
        {
            var score = testSkill.Score;

            if (score >= GoldScoreThreshold)
            {
                return GoldExperiencePoints;
            }

            if (score >= SilverScoreThreshold)
            {
                return SilverExperiencePoints;
            }

            if (score >= BronzeScoreThreshold)
            {
                return BronzeExperiencePoints;
            }

            return ParticipantExperiencePoints;
        }

        /// <summary>
        /// Gets the achieved date formatted as dd.MM.yyyy for display purposes.
        /// </summary>
        /// <param name="skillTest"></param>
        public static string AchievedDateFormatted(SkillTest skillTest)
        {
            return skillTest.AchievedDate.ToString("dd.MM.yyyy");
        }
    }
}
