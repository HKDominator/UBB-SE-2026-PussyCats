using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Services;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.ViewModels
{
    public class SkillTestCardViewModel : INotifyPropertyChanged
    {
        private SkillTest skillTest;
        private Badge badge;
        private ISkillTestService skillTestService;
        private UserProfileViewModel userProfileViewModel;
        private bool isRetakeEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public SkillTest SkillTest
        {
            get { return skillTest; }
        }
        public Badge Badge
        {
            get { return badge; }
        }
        public bool IsRetakeEnabled
        {
            get { return isRetakeEnabled; }
        }
        public SkillTestCardViewModel(SkillTest skillTest, ISkillTestService skillTestService, UserProfileViewModel userProfileViewModel)
        {
            this.skillTest = skillTest;
            this.skillTestService = skillTestService;
            this.userProfileViewModel = userProfileViewModel;

            CheckRetakeEligible();
            badge = SimpleModelOperations.AssignTier(skillTest.Score);
        }

        public void LoadCardCommand()
        {
            CheckRetakeEligible();
            UpdateBadge();
        }
        public void CheckRetakeEligible()
        {
            isRetakeEnabled = skillTestService.CanRetakeTest(skillTest.SkillTestId);
        }

        public void RetakeCommand()
        {
            if (!isRetakeEnabled)
            {
                return;
            }

            int newTestScore = Helpers.GenerateRandomScore(SkillTest.MinimumScore, SkillTest.MaximumScore);

            badge = skillTestService.SubmitRetake(skillTest.SkillTestId, newTestScore);

            skillTest.AchievedDate = DateOnly.FromDateTime(DateTime.Now);
            skillTest.Score = newTestScore;

            CheckRetakeEligible();
            UpdateBadge();

            OnPropertyChanged(nameof(Badge));
            OnPropertyChanged(nameof(SkillTest));
            OnPropertyChanged(nameof(IsRetakeEnabled));
            userProfileViewModel.RecalculateLevelCommand();
        }

        public void UpdateBadge()
        {
            badge = SimpleModelOperations.AssignTier(skillTest.Score);
        }
    }
}
