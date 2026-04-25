using System;
using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.Models;

namespace PussyCatsApp.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly List<Skill> skills = new List<Skill>();

        public Skill Load(int skillId)
        {
            foreach (Skill currentSkill in skills)
            {
                if (currentSkill.SkillId == skillId)
                {
                    return currentSkill;
                }
            }
            return null;
        }

        public void Save(int targetSkillId, Skill providedSkillData)
        {
            Skill skillFoundInStorage = null;
            foreach (Skill currentSkill in skills)
            {
                if (currentSkill.SkillId == targetSkillId)
                {
                    skillFoundInStorage = currentSkill;
                    break;
                }
            }

            if (skillFoundInStorage != null)
            {
                skillFoundInStorage.Name = providedSkillData.Name;
                skillFoundInStorage.Score = providedSkillData.Score;
                skillFoundInStorage.UserId = providedSkillData.UserId;
                skillFoundInStorage.AchievedDate = providedSkillData.AchievedDate;
            }
            else
            {
                providedSkillData.SkillId = targetSkillId;
                skills.Add(providedSkillData);
            }
        }

        public List<Skill> GetSkillsByUserId(int userId)
        {
            List<Skill> userSkills = new List<Skill>();
            foreach (Skill currentSkill in skills)
            {
                if (currentSkill.UserId == userId)
                {
                    userSkills.Add(currentSkill);
                }
            }
            return userSkills;
        }

        public void AddSkill(Skill newSkill)
        {
            const int NewSkillIdPlaceholder = 0;
            const int InitialIdValue = 1;
            const int IdIncrement = 1;

            if (newSkill.SkillId == NewSkillIdPlaceholder)
            {
                if (skills.Count == 0)
                {
                    newSkill.SkillId = InitialIdValue;
                }
                else
                {
                    int highestIdFound = 0;
                    foreach (Skill currentSkill in skills)
                    {
                        if (currentSkill.SkillId > highestIdFound)
                        {
                            highestIdFound = currentSkill.SkillId;
                        }
                    }
                    newSkill.SkillId = highestIdFound + IdIncrement;
                }
            }
            skills.Add(newSkill);
        }

        public void RemoveSkill(int skillId)
        {
            Skill skillToRemove = null;
            foreach (Skill currentSkill in skills)
            {
                if (currentSkill.SkillId == skillId)
                {
                    skillToRemove = currentSkill;
                    break;
                }
            }

            if (skillToRemove != null)
            {
                skills.Remove(skillToRemove);
            }
        }

        public void UpdateSkillScore(int skillId, double newScore)
        {
            Skill skillToUpdate = null;
            foreach (Skill currentSkill in skills)
            {
                if (currentSkill.SkillId == skillId)
                {
                    skillToUpdate = currentSkill;
                    break;
                }
            }

            if (skillToUpdate != null)
            {
                skillToUpdate.Score = newScore;
            }
        }
    }
}