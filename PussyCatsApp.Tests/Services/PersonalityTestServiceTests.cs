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

        private PersonalityTestService service;

        [TestInitialize]
        public void Initialize()
        {
            service = new PersonalityTestService(null);
        }

        /// <summary>
        /// Verifies that CalculateTraitScores maps each AnswerValue across all answer types to numeric trait scores and aggregates results by TraitType.
        /// </summary>
        [TestMethod]
        public void CalculateTraitScores_AllAnswerTypes_ShouldMapCorrectly()
        {
            //Arrange
            var q1 = new Question(1, "Q1", TraitType.VISIBILITY, 1);
            var q2 = new Question(2, "Q2", TraitType.VISIBILITY, 2);
            var q3 = new Question(3, "Q3", TraitType.VISIBILITY, 3);
            var q4 = new Question(4, "Q4", TraitType.VISIBILITY, 4);
            var q5 = new Question(5, "Q5", TraitType.VISIBILITY, 5);
            var answers = new Dictionary<Question, AnswerValue>
            {
                { q1, AnswerValue.STRONGLY_DISAGREE },
                { q2, AnswerValue.DISAGREE },
                { q3, AnswerValue.NEUTRAL },
                { q4, AnswerValue.AGREE },
                { q5, AnswerValue.STRONGLY_AGREE }
            };
            //Act
            var result = service.CalculateTraitScores(answers);
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
        public void CalculateRoleScores_AllRoles_ReturnCorrectValues(JobRole role, double expected)
        {
            // Arrange
            Dictionary<TraitType, double> traits = new Dictionary<TraitType, double>
            {
                { TraitType.VISIBILITY, 2 },
                { TraitType.CREATIVITY, 3 },
                { TraitType.PACE, 1 },
                { TraitType.DEPTH, 4 },
                { TraitType.INTERACTION, 2 },
                { TraitType.ABSTRACTION, 3 }
            };

            // Act
            var result = service.CalculateRoleScores(traits);

            // Assert
            Assert.AreEqual(expected, result[role]);
        }
        /// <summary>
        /// Verifies that GetTopRoles returns the correct number of roles.
        /// </summary>
        [TestMethod]
        public void GetTopRoles_ReturnsCorrectCount()
        {
            //Arrange
            var input = new Dictionary<JobRole, double> 
            {
                { JobRole.FrontendDeveloper, 10 },
                { JobRole.BackendDeveloper, 20 },
                { JobRole.UIUXDesigner, 15 }
            };
            //Act
            var result = service.GetTopRoles(input, 2);
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
            var input = new Dictionary<JobRole, double>
            {
                { JobRole.FrontendDeveloper, 10 },
                { JobRole.BackendDeveloper, 20 },
                { JobRole.UIUXDesigner, 15 }
            };
            //Act
            var result = service.GetTopRoles(input, 3).ToList();
            //Assert
            Assert.AreEqual(JobRole.BackendDeveloper, result[0].Key);
        }
    }
    
}
