using Moq;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;
using Match = PussyCatsApp.Models.Match;


namespace PussyCatsApp.Tests.Services;

[TestClass]
public class MatchServiceTest
{
    private Mock<IMatchRepository> mockRepo;
    private MatchService service;
    private List<Match> matches;

    [TestInitialize]
    public void Initialize()
    {
        mockRepo = new Mock<IMatchRepository>();
        service = new MatchService(mockRepo.Object);
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
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
        var result = service.GetMatchStatistics(1);
        Assert.AreEqual(3, result.TotalMatches);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectLastMonthCount()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
        var result = service.GetMatchStatistics(1);
        Assert.AreEqual(1, result.MatchesLastMonth);
    }

    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsCorrectSixMonthCount()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);
        var result = service.GetMatchStatistics(1);
        Assert.AreEqual(2, result.MatchesLastSixMonths);
    }

    [TestMethod]
    public void LastYearMatches_ShouldBeCorrect()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(3, result.MatchesLastYear);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsFrontendCount()
    {
        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(1, result.MatchesPerPosition["Frontend"]);
    }
    [TestMethod]
    public void GetStatistics_UserWithMatches_ReturnsBackendCount()
    {

        mockRepo.Setup(r => r.GetMatchesByUserId(1)).Returns(matches);

        var result = service.GetMatchStatistics(1);

        Assert.AreEqual(2, result.MatchesPerPosition["Backend"]);
    }

}

