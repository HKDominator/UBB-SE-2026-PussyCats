using Moq;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using Match = PussyCatsApp.Models.Match;


namespace PussyCatsApp.Tests.Services;

[TestClass]
public class MatchServiceTest
{
    private Mock<IMatchRepository> mockMatchRepository;
    private MatchService matchService;
    private List<Match> matches;

    [TestInitialize]
    public void Initialize()
    {
        mockMatchRepository = new Mock<IMatchRepository>();
        matchService = new MatchService(mockMatchRepository.Object);
        DateTime tenDaysAgo = DateTime.Now.AddDays(-10);
        DateTime eightMonthsAgo = DateTime.Now.AddMonths(-8);
        DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
        matches = new List<Match>
        {
            new Match { JobRole = "Backend", MatchDate = DateTime.Now.AddDays(-10) },
            new Match { JobRole = "Frontend", MatchDate = threeMonthsAgo },
            new Match { JobRole = "Backend", MatchDate = eightMonthsAgo }
        };
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsTotalCount()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);
        var result = matchService.GetMatchStatistics(1);
        Assert.AreEqual(3, result.TotalMatches);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectLastMonthCount()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);
        var result = matchService.GetMatchStatistics(1);
        Assert.AreEqual(1, result.MatchesLastMonth);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectSixMonthCount()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);
        var result = matchService.GetMatchStatistics(userId);
        Assert.AreEqual(2, result.MatchesLastSixMonths);
    }

    [TestMethod]
    public void LastYearMatches_ShouldBeCorrect()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);

        var result = matchService.GetMatchStatistics(userId);

        Assert.AreEqual(3, result.MatchesLastYear);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsFrontendCount()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);

        var result = matchService.GetMatchStatistics(userId);

        Assert.AreEqual(1, result.MatchesPerPosition["Frontend"]);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsBackendCount()
    {
        int userId = 1;
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(userId)).Returns(matches);

        var result = matchService.GetMatchStatistics(userId);

        Assert.AreEqual(2, result.MatchesPerPosition["Backend"]);
    }

}

