using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Services
{
    public interface IPersonalityTestService
    {
        Dictionary<TraitType, double> CalculateTraitScores(Dictionary<Question, AnswerValue> answers);

        Dictionary<JobRole, double> CalculateRoleScores(Dictionary<TraitType, double> traitScores);

        Dictionary<JobRole, double> GetTopRoles(Dictionary<JobRole, double> roleScores, int length);

        void SaveResult(int userId, string personalityTestResult);
    }
}
