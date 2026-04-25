using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class SkillRepositoryTests
    {
        private SkillRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            Repository=new SkillRepository();
        }

        [TestMethod]
        public void Load_SkillExists_ExpectsSkillReturned()
        {
            int TaRgetSkillId = 10;

            Skill skill = new Skill();
            skill.SkillId = TaRgetSkillId;
            Repository.AddSkill(skill);
                
            Skill result = Repository.Load(10);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Load_SkillDoesNotExist_ExpectsNull()
        {
            int NonIxistingId = 999;
            Skill result = Repository.Load(NonIxistingId);

            Assert.IsNull(result);
        }


        [TestMethod]
        public void Save_SkillExists_ExpectsUpdatedExistingData()
        {
            int ExistingId = 1;
            double NewScore = 85.0;

            Skill initial = new Skill();
            initial.SkillId = ExistingId;
            initial.Name = "Old Name";
            Repository.AddSkill(initial);

            Skill newData = new Skill();
            newData.Name = "New Name";
            newData.Score = NewScore;

            Repository.Save(ExistingId, newData);

            Skill updated = Repository.Load(ExistingId);
            Assert.AreEqual("New Name", updated.Name);
        }

        [TestMethod]
        public void Save_SkillDoesNotExist_ExcpectsNewSkillAdded()
        {
            int NewIdToAssign = 50;
            Skill newData = new Skill();
            newData.Name = "Brand New";

            Repository.Save(NewIdToAssign, newData);

            Skill result = Repository.Load(NewIdToAssign);
            Assert.AreEqual("Brand New", result.Name);
        }

        [TestMethod]
        public void AddSkill_FirstSkill_ExpectsIdOne()
        {
            int PlaceholderId = 0;
            int ExpectedFirstId = 1;

            Skill skill = new Skill();
            skill.SkillId = PlaceholderId; 

            Repository.AddSkill(skill);

            Assert.AreEqual(ExpectedFirstId, skill.SkillId);
        }

        [TestMethod]
        public void AddSkill_ExistingSkills_ExpectsMaximumIdPlusOne()
        {
            int InitialId = 10;
            int NewSkillPlaceholderId = 0;
            int ExpectedNextId = 11;

            Skill first = new Skill();
            first.SkillId = InitialId;
            Repository.AddSkill(first);

            Skill second = new Skill();
            second.SkillId = NewSkillPlaceholderId; 

            Repository.AddSkill(second);

            Assert.AreEqual(ExpectedNextId, second.SkillId);
        }


        [TestMethod]
        public void GetSkillsByUserId_UserHasSkills_ReturnsCorrectList()
        {
            int TargetUserId = 1;
            int OtherUserId = 2;
            int ExpectedCount = 2;
            Skill s1 = new Skill(); s1.UserId = TargetUserId; Repository.AddSkill(s1);
            Skill s2 = new Skill(); s2.UserId = TargetUserId; Repository.AddSkill(s2);
            Skill s3 = new Skill(); s3.UserId = OtherUserId; Repository.AddSkill(s3);

            List<Skill> results = Repository.GetSkillsByUserId(1);

            Assert.AreEqual(ExpectedCount, results.Count);
        }


        [TestMethod]
        public void RemoveSkill_SkillExists_RemovesFromList()
        {
            int SkillToRemove = 5;

            Skill skill = new Skill();
            skill.SkillId = SkillToRemove;
            Repository.AddSkill(skill);

            Repository.RemoveSkill(SkillToRemove);

            Assert.IsNull(Repository.Load(SkillToRemove));
        }

        [TestMethod]
        public void RemoveSkill_DoesNotExist_ExpectsExceptionHandled()
        {
            int NonExistentId = 999;
            Repository.RemoveSkill(NonExistentId);
        }


        [TestMethod]
        public void UpdateSkillScore_SkillExists_ExpectsNewScore()
        {
            int TargetSkillId = 1;
            double OriginalScore = 10.0;
            double NewTestScore = 95.5;

            Skill skill = new Skill();
            skill.SkillId = TargetSkillId;
            skill.Score = OriginalScore;
            Repository.AddSkill(skill);

            Repository.UpdateSkillScore(TargetSkillId, NewTestScore);

            Assert.AreEqual(95.5, Repository.Load(TargetSkillId).Score);
        }

        [TestMethod]
        public void UpdateSkillScore_DoesNotExist_ExcpectsExceptionHandled()
        {
            int MissingSkillId = 999;
            double TargetScore = 100.9;

            Repository.UpdateSkillScore(MissingSkillId, TargetScore);
        }
    }
}

