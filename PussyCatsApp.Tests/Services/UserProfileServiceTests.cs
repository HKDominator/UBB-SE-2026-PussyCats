using Moq;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services;

[TestClass]
public class UserProfileServiceTest
{
    private Mock<ISkillTestRepository> mockSkillRepo;
    private Mock<IUserProfileRepository> mockUserRepo;
    private UserProfileService service;
    
    [TestInitialize]
    public void Initialize()
    {
        mockSkillRepo = new Mock<ISkillTestRepository>();
        mockUserRepo = new Mock<IUserProfileRepository>();
        service = new UserProfileService(mockSkillRepo.Object, mockUserRepo.Object);
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

        var result = service.GenerateParsedCVText(profile);

        Assert.IsTrue(result.Contains("Ana Pop"));
        Assert.IsTrue(result.Contains("UBB"));
        Assert.IsTrue(result.Contains("React, CSS"));
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

        var result = service.GenerateParsedCVText(profile);

        var expected =
            "Ana Pop\n" +
            "\n" +           
            "";

        Assert.AreEqual(expected.TrimEnd(), result);
    }

    [TestMethod]
    public void GenerateParsedCVText_NullProfile_ReturnsEmpty()
    {
        var result = service.GenerateParsedCVText(null);

        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void IsProfileAvailable_ActiveAccount_ReturnsTrue()
    {

        var profile = new UserProfile
        {
            UserId = 1,
            ActiveAccount = true
        };

        mockUserRepo.Setup(userProfileById => userProfileById.GetProfileById(1)).Returns(profile);

        var result = service.IsProfileAvailable(1);

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

        mockUserRepo.Setup(userProfileById => userProfileById.GetProfileById(1)).Returns(profile);

        var result = service.IsProfileAvailable(1);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void IsProfileAvailable_ProfileNotFound_ThrowsException()
    {

        mockUserRepo.Setup(nullProfileByUserId => nullProfileByUserId.GetProfileById(1)).Returns((UserProfile)null);

        service.IsProfileAvailable(1);
    }

    [TestMethod]
    public void ToggleAccountStatus_FromActive_UpdatesToInactive()
    {
        service.ToggleAccountStatus(1, "ACTIVE");

        mockUserRepo.Verify(updateStatusForUser => updateStatusForUser.UpdateAccountStatus(1, "INACTIVE"), Times.Once);
        mockUserRepo.Verify(updateProfileLastModified => updateProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }

    [TestMethod]
    public void ToggleAccountStatus_FromInactive_UpdatesToActive()
    {
        service.ToggleAccountStatus(1, "INACTIVE");

        mockUserRepo.Verify(updateStatusForUser => updateStatusForUser.UpdateAccountStatus(1, "ACTIVE"), Times.Once);
        mockUserRepo.Verify(updateProfileLastModified => updateProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }
    [TestMethod]
    public void SaveProfile_ValidProfile_SetsParsedCvAndSaves()
    {
        var profile = new UserProfile
        {
            FirstName = "Ana",
            LastName = "Pop",
            University = "UBB",
            Skills = new List<string> { "C#", "SQL" }
        };

        service.SaveProfile(1, profile);

        Assert.IsTrue(profile.ParsedCV.Contains("Ana Pop"));
        Assert.IsTrue(profile.ParsedCV.Contains("UBB"));
        Assert.IsTrue(profile.ParsedCV.Contains("C#, SQL"));

        mockUserRepo.Verify(saveProfile => saveProfile.Save(1, profile), Times.Once);
        mockUserRepo.Verify(updateProfileLastModified => updateProfileLastModified.UpdateProfileLastModified(1, It.IsAny<DateTime>()), Times.Once);
    }
   
    [TestMethod]
    public void RecalculateLevel_NullProfile_ReturnsZero()
    {
        var result = service.RecalculateLevel(null);

        Assert.AreEqual(0, result);
    }
    [TestMethod]
    public void RecalculateLevel_WithSkillTests_ReturnsTotalXp()
    {

        var tests = new List<SkillTest>
    {
        new SkillTest(1, 1, "Test1", 95), 
        new SkillTest(2, 1, "Test2", 75)  
    };

        mockSkillRepo.Setup(skillTestsForUsers => skillTestsForUsers.GetSkillTestsByUserId(1))
                     .Returns(tests);

        var profile = new UserProfile { UserId = 1 };

        var result = service.RecalculateLevel(profile);

        Assert.AreEqual(160, result); 
    }

}
