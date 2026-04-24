using System;
using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;

namespace PussyCatsApp.Services
{
    public class MatchService : IMatchService
    {
        private const int LastMonth = 1;
        private const int LastSixMonths = 6;
        private const int LastYear = 12;

        private readonly IMatchRepository matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        private int CountMatchesInLastMonths(List<Match> matches, int months)
        {
            DateTime cutoffDate = DateTime.Now.AddMonths(-months);
            int matchesInPeriodCount = 0;

            foreach (var match in matches)
            {
                if (match.MatchDate > cutoffDate)
                {
                    matchesInPeriodCount++;
                }
            }

            return matchesInPeriodCount;
        }

        private Dictionary<string, int> GroupMatchesByPosition(List<Match> matches)
        {
            var positionCounts = new Dictionary<string, int>();

            foreach (var match in matches)
            {
                if (positionCounts.ContainsKey(match.JobRole))
                {
                    positionCounts[match.JobRole]++;
                }
                else
                {
                    positionCounts.Add(match.JobRole, 1);
                }
            }

            return positionCounts;
        }

        public List<Match> GetMatchesForUser(int userId)
        {
            return matchRepository.GetMatchesByUserId(userId);
        }

        public MatchStatistics GetMatchStatistics(int userId)
        {
            var matches = matchRepository.GetMatchesByUserId(userId);
            var matchStatistics = new MatchStatistics();

            matchStatistics.TotalMatches = matches.Count;
            matchStatistics.MatchesLastMonth = CountMatchesInLastMonths(matches, LastMonth);
            matchStatistics.MatchesLastSixMonths = CountMatchesInLastMonths(matches, LastSixMonths);
            matchStatistics.MatchesLastYear = CountMatchesInLastMonths(matches, LastYear);
            matchStatistics.MatchesPerPosition = GroupMatchesByPosition(matches);

            return matchStatistics;
        }
    }
}