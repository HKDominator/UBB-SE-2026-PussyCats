using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PussyCatsApp.Converters;
using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    /// <summary>
    /// ViewModel for managing the personality test workflow, including loading questions,
    /// handling user answers, submitting the test, and saving the selected role result.
    /// </summary>
    public partial class PersonalityTestViewModel : ObservableObject
    {
        private readonly IPersonalityTestService personalityTestService;
        private readonly int userId;
        private const int NumberOfTopRolesToDisplay = 3;

        public List<QuestionViewModel> Questions { get; }

        [ObservableProperty]
        public partial List<RoleResultViewModel> TopRoles { get; set; } = new ();

        [ObservableProperty]
        public partial RoleResultViewModel? SelectedRole { get; set; }

        [ObservableProperty]
        public partial string? SaveMessage { get; set; }

        [ObservableProperty]
        public partial bool IsTestSubmitted { get; set; }

        public bool CanSubmit
        {
            get
            {
                foreach (var question in Questions)
                {
                    if (question.IsAnswered == false)
                    {
                        return false; // At least one question is unanswered
                    }
                }
                return true;
            }
        }

        public bool CanSave
        {
            get
            {
                return SelectedRole != null;
            }
        }

        public PersonalityTestViewModel(int userId, IPersonalityTestService personalityTestService)
        {
            this.personalityTestService = personalityTestService;
            this.userId = userId;

            var rawQuestionsList = PersonalityTestService.LoadQuestions();
            Questions = WrapQuestionsInViewModels(rawQuestionsList);
            SubscribeToAnswerChanges();
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            var answers = CollectAnswersFromQuestions();
            var traitScores = personalityTestService.CalculateTraitScores(answers);
            var roleScores = personalityTestService.CalculateRoleScores(traitScores);

            var topRolesDictionary = personalityTestService.GetTopRoles(roleScores, NumberOfTopRolesToDisplay);
            TopRoles = WrapRolesInViewModels(topRolesDictionary);

            IsTestSubmitted = true;
        }

        private Dictionary<Question, AnswerValue> CollectAnswersFromQuestions()
        {
            var answers = new Dictionary<Question, AnswerValue>();
            foreach (QuestionViewModel questionViewModel in Questions)
            {
                answers[questionViewModel.Question] = (AnswerValue)questionViewModel.SelectedAnswer!;
            }
            return answers;
        }

        private static List<RoleResultViewModel> WrapRolesInViewModels(Dictionary<JobRole, double> rolesDictionary)
        {
            var roleViewModels = new List<RoleResultViewModel>();
            foreach (KeyValuePair<JobRole, double> rolePair in rolesDictionary)
            {
                roleViewModels.Add(new RoleResultViewModel(rolePair.Key, rolePair.Value));
            }
            return roleViewModels;
        }

        [RelayCommand]
        private void SelectRole(RoleResultViewModel roleResultViewModel)
        {
            // Deselect all roles
            foreach (var topRoleViewModel in TopRoles)
            {
                topRoleViewModel.IsSelected = false;
            }

            // Select the clicked role
            roleResultViewModel.IsSelected = true;
            SelectedRole = roleResultViewModel;

            // Notify SaveResultCommand that it can execute
            SaveResultCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void SaveResult()
        {
            if (SelectedRole != null)
            {
                personalityTestService.SaveResult(this.userId, SelectedRole.Role.ToString());

                // Convert role to a friendly display name using the existing jobRoleToDisplayNameConverter
                var jobRoleToDisplayNameConverter = new JobRoleToDisplayNameConverter();
                string language = "en-US";
                var displayNameObject = jobRoleToDisplayNameConverter.Convert(SelectedRole.Role, typeof(string), null, language);
                var displayName = displayNameObject as string;
                if (displayName == null)
                {
                    SelectedRole.Role.ToString();
                }

                // Notify the view that the result was saved so it can display feedback
                SaveMessage = $"Your personality test result has been updated to {displayName}.";
            }
        }

        private static List<QuestionViewModel> WrapQuestionsInViewModels(List<Question> rawQuestionsList)
        {
            var questionViewModels = new List<QuestionViewModel>();
            foreach (Question question in rawQuestionsList)
            {
                questionViewModels.Add(new QuestionViewModel(question));
            }
            return questionViewModels;
        }

        private void SubscribeToAnswerChanges()
        {
            foreach (QuestionViewModel questionViewModel in Questions)
            {
                questionViewModel.PropertyChanged += OnQuestionAnswerChanged;
            }
        }

        private void OnQuestionAnswerChanged(object? sender, PropertyChangedEventArgs eventArguments)
        {
            if (eventArguments.PropertyName == nameof(QuestionViewModel.SelectedAnswer))
            {
                SubmitCommand.NotifyCanExecuteChanged();
            }
        }
    }
}