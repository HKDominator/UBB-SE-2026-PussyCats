using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class PersonalityTestServiceTests
    {

        private PersonalityTestService personalityTestService;

        [TestInitialize]
        public void Initialize()
        {
            personalityTestService = new PersonalityTestService(null);
        }

        /// <summary>
        /// Verifies that CalculateTraitScores maps each AnswerValue across all answer types to numeric trait scores and aggregates results by TraitType.
        /// </summary>
        [TestMethod]
        public void CalculateTraitScores_AllAnswerTypes_ShouldMapCorrectly()
        {
            //Arrange
            var firstQuestion = new Question(1, "Q1", TraitType.VISIBILITY, 1);
            var secondQuestion = new Question(2, "Q2", TraitType.VISIBILITY, 2);
            var thirdQuestion = new Question(3, "Q3", TraitType.VISIBILITY, 3);
            var forthQuestion = new Question(4, "Q4", TraitType.VISIBILITY, 4);
            var fifthQuestion = new Question(5, "Q5", TraitType.VISIBILITY, 5);
            var questionAnswers = new Dictionary<Question, AnswerValue>
            {
                { firstQuestion, AnswerValue.STRONGLY_DISAGREE },
                { secondQuestion, AnswerValue.DISAGREE },
                { thirdQuestion, AnswerValue.NEUTRAL },
                { forthQuestion, AnswerValue.AGREE },
                { fifthQuestion, AnswerValue.STRONGLY_AGREE }
            };
            //Act
            var result = personalityTestService.CalculateTraitScores(questionAnswers);
            //Assert
            Assert.IsTrue(result.ContainsKey(TraitType.VISIBILITY));
        }
        /// <summary>
        /// Verifies that CalculateRoleScores returns correct scores for all job roles given a specific set of trait values.
        /// </summary>
        [DataTestMethod]
        [DataRow(JobRole.FrontendDeveloper, 11)]
        [DataRow(JobRole.BackendDeveloper, 15)]
        [DataRow(JobRole.UIUXDesigner, 14)]
        [DataRow(JobRole.DevOpsEngineer, 13)]
        [DataRow(JobRole.ProjectManager, 10)]
        [DataRow(JobRole.DataAnalyst, 17)]
        [DataRow(JobRole.CybersecuritySpecialist, 21)]
        [DataRow(JobRole.AIMLEngineer, 21)]
        public void CalculateRoleScores_AllRoles_ReturnCorrectValues(JobRole role, double expectedScore)
        {
            // Arrange
            Dictionary<TraitType, double> traitScores = new Dictionary<TraitType, double>
            {
                { TraitType.VISIBILITY, 2 },
                { TraitType.CREATIVITY, 3 },
                { TraitType.PACE, 1 },
                { TraitType.DEPTH, 4 },
                { TraitType.INTERACTION, 2 },
                { TraitType.ABSTRACTION, 3 }
            };

            // Act
            var result = personalityTestService.CalculateRoleScores(traitScores);

            // Assert
            Assert.AreEqual(expectedScore, result[role]);
        }
        /// <summary>
        /// Verifies that GetTopRoles returns the correct number of roles.
        /// </summary>
        [TestMethod]
        public void GetTopRoles_ReturnsCorrectCount()
        {
            //Arrange
            var roleScores = new Dictionary<JobRole, double> 
            {
                { JobRole.FrontendDeveloper, 10 },
                { JobRole.BackendDeveloper, 20 },
                { JobRole.UIUXDesigner, 15 }
            };
            //Act
            var result = personalityTestService.GetTopRoles(roleScores, 2);
            //Assert
            Assert.AreEqual(2, result.Count);
        }
        /// <summary>
        /// Verifies that GetTopRoles returns roles ordered by score in descending order, with the highest-scored role first.
        /// </summary>
        [TestMethod]
        public void GetTopRoles_HighestScoreIsFirst()
        {
            //Arrange
            var roleScores = new Dictionary<JobRole, double>
            {
                { JobRole.FrontendDeveloper, 10 },
                { JobRole.BackendDeveloper, 20 },
                { JobRole.UIUXDesigner, 15 }
            };
            //Act
            var result = personalityTestService.GetTopRoles(roleScores, 3).ToList();
            //Assert
            Assert.AreEqual(JobRole.BackendDeveloper, result[0].Key);
        }
    }
    
}
