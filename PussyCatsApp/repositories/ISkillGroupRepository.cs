using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Repositories
{
    public interface ISkillGroupRepository
    {
        List<SkillGroup> GetSkillsGroupByRole(JobRole role);
    }
}