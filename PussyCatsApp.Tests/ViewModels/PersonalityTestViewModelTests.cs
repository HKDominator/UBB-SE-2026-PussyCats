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
        public void TestInitializesWithQuestions()
        {
            int numberOfQuestions = 24; // Assuming the test has 24 questions
            Assert.IsTrue(viewModel.Questions.Count == numberOfQuestions);
        }

        [TestMethod]
        public void TestCanSubmitWhenAllQuestionsAreAnswered()
        {
            var answeredQuestion = new QuestionViewModel(new Question(1, "Does this work?", TraitType.ABSTRACTION, 0))
            {
                SelectedAnswer = (int)AnswerValue.AGREE
            };

            viewModel.Questions.Clear();
            viewModel.Questions.Add(answeredQuestion);

            Assert.IsTrue(viewModel.CanSubmit);
        }

        [TestMethod]
        public void TestCannotSubmitWhenAtLeastOneQuestionIsUnanswered()
        {
            var answeredQuestion = new QuestionViewModel(new Question(1, "Does this work?", TraitType.ABSTRACTION, 0))
            {
                SelectedAnswer = (int)AnswerValue.AGREE
            };
            var unansweredQuestion = new QuestionViewModel(new Question(2, "Is this unanswered?", TraitType.ABSTRACTION, 0));
            viewModel.Questions.Clear();
            viewModel.Questions.Add(answeredQuestion);
            viewModel.Questions.Add(unansweredQuestion);
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void TestCannotSaveWhenNoRoleSelected()
        {
            viewModel.SelectedRole = null;
            Assert.IsFalse(viewModel.CanSave);
        }

        [TestMethod]
        public void TestCanSaveWhenRoleSelected()
        {
            viewModel.SelectedRole = new RoleResultViewModel(JobRole.DataAnalyst, 0.9);
            Assert.IsTrue(viewModel.CanSave);
        }
    }
}
