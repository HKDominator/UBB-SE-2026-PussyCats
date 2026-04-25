using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Services;

namespace PussyCatsApp.ViewModels
{
    public class MatchHistoryViewModel
    {
        private List<Match> matches;
        private MatchStatistics statistics;
        private string errorMessage;
        private int currentUserId;
        private IMatchService matchService;

        public MatchHistoryViewModel(int userId, IMatchService matchService)
        {
            this.matchService = matchService;
            currentUserId = userId;
            matches = new List<Match>();
            errorMessage = string.Empty;
        }

        public void LoadMatches()
        {
            errorMessage = string.Empty;
            try
            {
                matches = matchService.GetMatchesForUser(currentUserId);
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        public void LoadStatistics()
        {
            errorMessage = string.Empty;
            try
            {
                statistics = matchService.GetMatchStatistics(currentUserId);
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        public List<Match> GetMatches()
        {
            return matches;
        }

        public MatchStatistics GetStatistics()
        {
            return statistics;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}
