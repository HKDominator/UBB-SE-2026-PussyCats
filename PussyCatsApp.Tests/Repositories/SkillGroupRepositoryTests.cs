using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class SkillGroupRepositoryTests
    {
 
        private SkillGroupRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
           Repository = new SkillGroupRepository();
        }

        [TestMethod]
        public void GetSkillsGroupByRole_FrontendDeveloper_ExpectsHTMLSkill()
        {
            var result = Repository.GetSkillsGroupByRole(JobRole.FrontendDeveloper);

            bool isHtmlSkillPresent = false;

            foreach(var group in result)
            {
                foreach(var skill in group.Skills) {
                    if (skill=="HTML")
                    {
                        isHtmlSkillPresent = true;
                        break;
                    }
                }
            }

            Assert.IsTrue(isHtmlSkillPresent, "The Frontend skills should include HTML");
        }

        [TestMethod]
        public void GetSkillsGroupByRole_DevOpsRole_ExpectsWeightedSumTo100()
        {
            var DevOopsGroup=Repository.GetSkillsGroupByRole(JobRole.DevOpsEngineer);

            int totalWeight = 0;

            foreach (var group in DevOopsGroup)
            {
                totalWeight += group.Weight;
            }
            Assert.AreEqual(100,totalWeight, "The sum of the weights should be 100");

        }
    }
}
