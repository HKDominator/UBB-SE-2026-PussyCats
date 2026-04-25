using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services;

[TestClass]
public class UserProfileServiceTest
{
    private Mock<ISkillTestRepository> mockSkillTestRepository;
    private Mock<IUserProfileRepository> mockUserProfileRepository;
    private UserProfileService userProfileService;
    
    [TestInitialize]
    public void Initialize()
    {
        mockSkillTestRepository = new Mock<ISkillTestRepository>();
        mockUserProfileRepository = new Mock<IUserProfileRepository>();
        userProfileService = new UserProfileService(mockSkillTestRepository.Object, mockUserProfileRepository.Object);
    }

    [TestMethod]
    public void GenerateParsedCVText_ValidProfile_ReturnsFormattedString()
    {

        var profile = new UserProfile
        {
            FirstName = "Ana",
            LastName = "Pop",
            University = "UBB",
            Skills = new List<string> { "React", "CSS" }
        };

        var parsedCV = userProfileService.GenerateParsedCVText(profile);

        Assert.IsTrue(parsedCV.Contains("Ana Pop"));
        Assert.IsTrue(parsedCV.Contains("UBB"));
        Assert.IsTrue(parsedCV.Contains("React, CSS"));
    }

    [TestMethod]
    public void GenerateParsedCVText_NullUniversityAndSkills_ReturnsEmptyLines()
    {

        var profile = new UserProfile
        {
            FirstName = "Ana",
            LastName = "Pop",
            University = null,
            Skills = null
        };

        var parsedCV = userProfileService.GenerateParsedCVText(profile);

        var expectedParsedCV =
            "Ana Pop\n" +
            "\n" +           
            "";

        Assert.AreEqual(expectedParsedCV.TrimEnd(), parsedCV);
    }

    [TestMethod]
    public void GenerateParsedCVText_NullProfile_ReturnsEmpty()
    {
        var resultForInexistentUserProfile = userProfileService.GenerateParsedCVText(null);

        Assert.AreEqual(string.Empty, resultForInexistentUserProfile);
    }

    [TestMethod]
    public void IsProfileAvailable_ActiveAccount_ReturnsTrue()
    {

        var profile = new UserProfile
        {
            UserId = 1,
            ActiveAccount = true
        };

        mockUserProfileRepository.Setup(findsUserProfile => findsUserProfile.GetProfileById(1)).Returns(profile);

        var result = userProfileService.IsProfileAvailable(1);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsProfileAvailable_InactiveAccount_ReturnsFalse()
    {

        var profile = new UserProfile
        {
            UserId = 1,
            ActiveAccount = false
        };

        mockUserProfileRepository.Setup(findsUserProfile => findsUserProfile.GetProfileById(1)).Returns(profile);

        var result = userProfileService.IsProfileAvailable(1);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void IsProfileAvailable_ProfileNotFound_ThrowsException()
    {

        mockUserProfileRepository.Setup(doesNotFindUserProfile => doesNotFindUserProfile.GetProfileById(1)).Returns((UserProfile)null);

        userProfileService.IsProfileAvailable(1);
    }

    [TestMethod]
    public void ToggleAccountStatus_FromActive_UpdatesToInactive()
    {
        var userId = 1;
        var accountStatus = "ACTIVE";
        userProfileService.ToggleAccountStatus(userId, accountStatus);

        mockUserProfileRepository.Verify(updatesAccountStatusToInactive => updatesAccountStatusToInactive.UpdateAccountStatus(1, "INACTIVE"), Times.Once);
        mockUserProfileRepository.Verify(updatesProfileLastModified => updatesProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }

    [TestMethod]
    public void ToggleAccountStatus_FromInactive_UpdatesToActive()
    {
        var userId = 1;
        var accountStatus = "INACTIVE";
        userProfileService.ToggleAccountStatus(userId, accountStatus);

        mockUserProfileRepository.Verify(updatesAccountStatusToActive => updatesAccountStatusToActive.UpdateAccountStatus(1, "ACTIVE"), Times.Once);
        mockUserProfileRepository.Verify(updatesProfileLastModified => updatesProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }
    [TestMethod]
    public void SaveProfile_ValidProfile_SetsParsedCvAndSaves()
    {
        var userId = 1;
        var profile = new UserProfile
        {
            FirstName = "Ana",
            LastName = "Pop",
            University = "UBB",
            Skills = new List<string> { "C#", "SQL" }
        };

        userProfileService.SaveProfile(userId, profile);

        Assert.IsTrue(profile.ParsedCV.Contains("Ana Pop"));
        Assert.IsTrue(profile.ParsedCV.Contains("UBB"));
        Assert.IsTrue(profile.ParsedCV.Contains("C#, SQL"));

        mockUserProfileRepository.Verify(savesProfile => savesProfile.Save(userId, profile), Times.Once);
        mockUserProfileRepository.Verify(updatesProfileLastModified => updatesProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }
   
    [TestMethod]
    public void RecalculateLevel_NullProfile_ReturnsZero()
    {
        var resultForInexistingUserProfile = userProfileService.RecalculateLevel(null);

        Assert.AreEqual(0, resultForInexistingUserProfile);
    }
    [TestMethod]
    public void RecalculateLevel_WithSkillTests_ReturnsTotalXp()
    {

        var tests = new List<SkillTest>
    {
        new SkillTest(1, 1, "Test1", 95), 
        new SkillTest(2, 1, "Test2", 75)  
    };

        mockSkillTestRepository.Setup(findsSkillTests => findsSkillTests.GetSkillTestsByUserId(1))
                     .Returns(tests);

        var profile = new UserProfile { UserId = 1 };

        var result = userProfileService.RecalculateLevel(profile);

        Assert.AreEqual(160, result); 
    }

}
