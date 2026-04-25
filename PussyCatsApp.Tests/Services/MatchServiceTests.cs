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
        matches = new List<Match>
        {
            new Match { JobRole = "Backend", MatchDate = DateTime.Now.AddDays(-10) },
            new Match { JobRole = "Frontend", MatchDate = DateTime.Now.AddMonths(-3) },
            new Match { JobRole = "Backend", MatchDate = DateTime.Now.AddMonths(-8) }
        };
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsTotalCount()
    {
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);
        var result = matchService.GetMatchStatistics(1);
        Assert.AreEqual(3, result.TotalMatches);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectLastMonthCount()
    {
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);
        var result = matchService.GetMatchStatistics(1);
        Assert.AreEqual(1, result.MatchesLastMonth);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectSixMonthCount()
    {
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);
        var result = matchService.GetMatchStatistics(1);
        Assert.AreEqual(2, result.MatchesLastSixMonths);
    }

    [TestMethod]
    public void LastYearMatches_ShouldBeCorrect()
    {
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);

        var result = matchService.GetMatchStatistics(1);

        Assert.AreEqual(3, result.MatchesLastYear);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsFrontendCount()
    {
        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);

        var result = matchService.GetMatchStatistics(1);

        Assert.AreEqual(1, result.MatchesPerPosition["Frontend"]);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsBackendCount()
    {

        mockMatchRepository.Setup(findsUserMatches => findsUserMatches.GetMatchesByUserId(1)).Returns(matches);

        var result = matchService.GetMatchStatistics(1);

        Assert.AreEqual(2, result.MatchesPerPosition["Backend"]);
    }

}

