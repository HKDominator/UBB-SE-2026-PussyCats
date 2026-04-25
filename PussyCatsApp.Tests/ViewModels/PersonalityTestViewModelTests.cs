using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class PersonalityTestViewModelTests
    {
        private Mock<IPersonalityTestService> mockPersonalityTestService;
        private PersonalityTestViewModel viewModel;
        private const int testUserId = 1;

        [TestInitialize]
        public void SetUp()
        {
            mockPersonalityTestService = new Mock<IPersonalityTestService>();
            viewModel = new PersonalityTestViewModel(testUserId, mockPersonalityTestService.Object);
        }

        [TestMethod]
        public void ViewModel_Initializes_WithDefaultNumberOfQuestions()
        {
            int numberOfQuestions = 24; // Assuming the test has 24 questions
            Assert.IsTrue(viewModel.Questions.Count == numberOfQuestions);
        }

        [TestMethod]
        public void CanSubmit_IsTrue_WhenAllQuestionsAreAnswered()
        {
            // The specific trait type and sort order are not important for this test, so we can use any valid values.
            const int unimportantSortOrder = 0;
            const TraitType unimportantTraitType = TraitType.ABSTRACTION;

            var answeredQuestion = new QuestionViewModel(new Question(1, "Does this work?", unimportantTraitType, unimportantSortOrder))
            {
                SelectedAnswer = (int)AnswerValue.AGREE
            };

            viewModel.Questions.Clear();
            viewModel.Questions.Add(answeredQuestion);

            Assert.IsTrue(viewModel.CanSubmit);
        }

        [TestMethod]
        public void CanSubmit_IsFalse_WhenAtLeastOneQuestionIsUnanswered()
        {
            // The specific trait type and sort order are not important for this test, so we can use any valid values.
            const int unimportantSortOrder = 0;
            const TraitType unimportantTraitType = TraitType.ABSTRACTION;
            int firstQuestionId = 1, secondQuestionId = 2;

            var answeredQuestion = new QuestionViewModel(new Question(firstQuestionId, "Does this work?", unimportantTraitType, unimportantSortOrder))
            {
                SelectedAnswer = (int)AnswerValue.AGREE
            };
            var unansweredQuestion = new QuestionViewModel(new Question(secondQuestionId, "Is this unanswered?", unimportantTraitType, unimportantSortOrder));
            viewModel.Questions.Clear();
            viewModel.Questions.Add(answeredQuestion);
            viewModel.Questions.Add(unansweredQuestion);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void CanSave_IsFalse_WhenNoRoleSelected()
        {
            viewModel.SelectedRole = null;
            Assert.IsFalse(viewModel.CanSave);
        }

        [TestMethod]
        public void CanSave_IsTrue_WhenRoleSelected()
        {
            double scoreValue = 0.9;
            viewModel.SelectedRole = new RoleResultViewModel(JobRole.DataAnalyst, scoreValue);
            Assert.IsTrue(viewModel.CanSave);
        }
    }
}
