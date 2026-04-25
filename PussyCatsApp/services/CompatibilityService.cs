using System;
using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class CompatibilityService : ICompatibilityService
    {
        private const int SkillsLineIndex = 2;
        private const char SkillDelimiter = ',';
        private const double UnverifiedSkillScore = 0.5;
        private const double ScoreNormalizationFactor = 100.0;
        private const double GapThreshold = 0.5;
        private const double TargetGroupScore = 0.8;
        private const int MaxSuggestions = 3;
        private const int InvalidScore = -1;

        private IUserSkillRepository userSkillRepository;
        private ISkillGroupRepository skillGroupRepository;

        public CompatibilityService(IUserSkillRepository userSkillRepository, ISkillGroupRepository skillGroupRepository)
        {
            this.userSkillRepository = userSkillRepository;
            this.skillGroupRepository = skillGroupRepository;
        }

        private List<UserSkill> GetUserSkills(int userId)
        {
            List<UserSkill> verifiedSkillsOfUser = userSkillRepository.GetVerifiedSkillsByUserId(userId);
            string parsedCvText = userSkillRepository.GetParsedCvByUserId(userId);
            List<string> extractedCvSkills = ExtractSkillsFromParsedCv(parsedCvText);
            return MergeVerifiedAndUnverifiedSkills(verifiedSkillsOfUser, extractedCvSkills);
        }

        private List<string> ExtractSkillsFromParsedCv(string parsedCv)
        {
            List<string> extractedSkills = new List<string>();

            if (string.IsNullOrWhiteSpace(parsedCv))
            {
                return extractedSkills;
            }

            string[] lines = parsedCv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length <= SkillsLineIndex)
            {
                return extractedSkills;
            }

            string skillsLine = lines[SkillsLineIndex].Trim();

            if (string.IsNullOrWhiteSpace(skillsLine))
            {
                return extractedSkills;
            }

            string[] cvSkills = skillsLine.Split(SkillDelimiter);

            foreach (string cvSkill in cvSkills)
            {
                string skillName = cvSkill.Trim();
                if (!string.IsNullOrWhiteSpace(skillName))
                {
                    extractedSkills.Add(skillName);
                }
            }

            return extractedSkills;
        }

        private List<UserSkill> MergeVerifiedAndUnverifiedSkills(List<UserSkill> verifiedSkills, List<string> cvSkills)
        {
            List<UserSkill> allSkills = new List<UserSkill>();

            foreach (UserSkill verifiedSkill in verifiedSkills)
            {
                allSkills.Add(verifiedSkill);
            }

            foreach (string cvSkill in cvSkills)
            {
                bool isSkillAlreadyInAllSkills = false;

                foreach (UserSkill existingSkill in allSkills)
                {
                    if (string.Equals(existingSkill.SkillName, cvSkill, StringComparison.OrdinalIgnoreCase))
                    {
                        isSkillAlreadyInAllSkills = true;
                        break;
                    }
                }

                if (!isSkillAlreadyInAllSkills)
                {
                    allSkills.Add(new UserSkill
                    {
                        SkillName = cvSkill,
                        IsVerified = false,
                        Score = 0
                    });
                }
            }

            return allSkills;
        }

        private double ComputeGroupScore(SkillGroup group, List<UserSkill> userSkills)
        {
            double maximumSkillScore = 0;

            foreach (string skill in group.Skills)
            {
                double skillScore = 0;

                foreach (UserSkill userSkill in userSkills)
                {
                    if (string.Equals(userSkill.SkillName, skill, StringComparison.OrdinalIgnoreCase))
                    {
                        skillScore = userSkill.IsVerified
                            ? userSkill.Score / ScoreNormalizationFactor
                            : UnverifiedSkillScore;

                        break;
                    }
                }

                if (skillScore > maximumSkillScore)
                {
                    maximumSkillScore = skillScore;
                }
            }

            return maximumSkillScore;
        }

        private double ComputeMatchScore(List<SkillGroup> groups, List<double> groupScores)
        {
            int totalWeight = 0;
            foreach (SkillGroup group in groups)
            {
                totalWeight += group.Weight;
            }

            if (totalWeight == 0)
            {
                return InvalidScore;
            }

            double weightedSum = 0;
            for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                weightedSum += groups[groupIndex].Weight * groupScores[groupIndex];
            }

            return weightedSum * ScoreNormalizationFactor / totalWeight;
        }

        private double ComputeGain(SkillGroup group, double groupScore, int totalWeight)
        {
            return ScoreNormalizationFactor * group.Weight * (TargetGroupScore - groupScore) / totalWeight;
        }
        private int CompareGains(Suggestion firstSuggestion, Suggestion secondSuggestion)
        {
            return secondSuggestion.GainScore.CompareTo(firstSuggestion.GainScore);
        }
        private bool UserHasSkill(List<UserSkill> userSkills, string skill)
        {
            foreach (UserSkill userSkill in userSkills)
            {
                if (string.Equals(userSkill.SkillName, skill, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        private List<Suggestion> IdentifyGaps(List<SkillGroup> groups, List<UserSkill> userSkills, int totalWeight)
        {
            List<Suggestion> suggestions = new List<Suggestion>();

            foreach (SkillGroup group in groups)
            {
                double groupScore = ComputeGroupScore(group, userSkills);

                if (groupScore > GapThreshold)
                {
                    continue;
                }

                Suggestion bestSuggestion = null;

                foreach (string skill in group.Skills)
                {
                    if (UserHasSkill(userSkills, skill))
                    {
                        continue;
                    }

                    double gain = ComputeGain(group, groupScore, totalWeight);

                    Suggestion suggestion = new Suggestion
                    {
                        SkillName = skill,
                        GroupName = group.GroupName,
                        GainScore = gain
                    };

                    bestSuggestion = suggestion;
                    break;
                }

                if (bestSuggestion != null)
                {
                    suggestions.Add(bestSuggestion);
                }
            }

            suggestions.Sort(CompareGains);

            if (suggestions.Count > MaxSuggestions)
            {
                suggestions = suggestions.GetRange(0, MaxSuggestions);
            }

            return suggestions;
        }

        public RoleResult CalculateForRole(int userId, JobRole role)
        {
            List<UserSkill> userSkills = GetUserSkills(userId);
            List<SkillGroup> groups = skillGroupRepository.GetSkillsGroupByRole(role);

            int totalWeight = 0;
            foreach (SkillGroup group in groups)
            {
                totalWeight += group.Weight;
            }

            List<double> groupScores = new List<double>();
            foreach (SkillGroup group in groups)
            {
                groupScores.Add(ComputeGroupScore(group, userSkills));
            }

            double matchScore = ComputeMatchScore(groups, groupScores);

            RoleResult result = new RoleResult();
            result.JobRole = role;

            if (matchScore == InvalidScore)
            {
                result.MatchScore = InvalidScore;
                result.Suggestions = new List<Suggestion>();
                return result;
            }

            result.MatchScore = matchScore;
            result.Suggestions = IdentifyGaps(groups, userSkills, totalWeight);

            return result;
        }

        public List<RoleResult> CalculateAll(int userId)
        {
            List<RoleResult> results = new List<RoleResult>();

            foreach (JobRole role in Enum.GetValues(typeof(JobRole)))
            {
                results.Add(CalculateForRole(userId, role));
            }

            return results;
        }

        public List<Suggestion> GetSuggestions(RoleResult result)
        {
            return result.Suggestions;
        }
    }
}