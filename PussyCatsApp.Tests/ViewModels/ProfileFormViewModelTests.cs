using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Services;
using PussyCatsApp.ViewModels;
using PussyCatsApp.Models;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class ProfileFormViewModelTests
    {
        private Mock<IUserProfileService> mockProfileService;
        private Mock<ICVParsingService> mockCVParsingService;
        private ProfileFormViewModel viewModel;

        [TestInitialize]
        public void Setup()
        {
            mockProfileService = new Mock<IUserProfileService>();
            mockCVParsingService = new Mock<ICVParsingService>();
            viewModel = new ProfileFormViewModel(mockProfileService.Object, mockCVParsingService.Object);
        }

        [TestMethod]
        public void AddSkill_AddsSkillToList_WhenValidSkillProvided()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);

            int expectedNumberOfSkills = 1;
            Assert.AreEqual(expectedNumberOfSkills, viewModel.Skills.Count);
            Assert.IsTrue(viewModel.Skills.Contains(skill));
        }

        [TestMethod]
        public void AddSkill_DoesNotAddToList_WhenSkillIsEmpty()
        {
            string skill = string.Empty;
            viewModel.AddSkill(skill);

            int expectedNumberOfSkills = 0;
            Assert.AreEqual(expectedNumberOfSkills, viewModel.Skills.Count);
        }

        [TestMethod]
        public void AddSkill_DoesNotAddToList_WhenDuplicateSkill()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            viewModel.AddSkill(skill);

            int expectedNumberOfSkills = 1;
            Assert.AreEqual(expectedNumberOfSkills, viewModel.Skills.Count);
        }

        [TestMethod]
        public void AddSkill_ShowsInfoBar_WhenDuplicateSkillAdded()
        {
            string skill = "C#";
            string duplicateStringInfoBarMessage = "This skill has already been added.";

            viewModel.AddSkill(skill);
            viewModel.AddSkill(skill);

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(duplicateStringInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void AddSkill_DoesNotAddToList_WhenMaximumNumberOfSkillsReached()
        {
            int maximumNumberOfSkillsAllowed = 30;
            string skill = "Skill";
            for (int skillIndex = 0; skillIndex < maximumNumberOfSkillsAllowed; skillIndex++)
            {
                viewModel.AddSkill(skill + skillIndex);
            }

            viewModel.AddSkill("ExtraSkill");
            Assert.AreEqual(maximumNumberOfSkillsAllowed, viewModel.Skills.Count);
        }

        [TestMethod]
        public void AddSkill_ShowsInfoBar_WhenMaximumNumberOfSkillsIsExceeded()
        {
            int maximumNumberOfSkillsAllowed = 30;
            string skill = "Skill";
            string maximumNumberOfSkillsInfoBarMessage = $"Maximum of {maximumNumberOfSkillsAllowed} skills allowed.";
            for (int skillIndex = 0; skillIndex < maximumNumberOfSkillsAllowed; skillIndex++)
            {
                viewModel.AddSkill(skill + skillIndex);
            }
            viewModel.AddSkill("ExtraSkill");
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(maximumNumberOfSkillsInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void AddSkill_DoesNotAddToList_WhenSkillNameExceedsMaximumLength()
        {
            int maximumSkillNameLength = 60;
            string longSkillName = new string('a', maximumSkillNameLength + 1);

            viewModel.AddSkill(longSkillName);

            int expectedNumberOfSkills = 0;
            Assert.AreEqual(expectedNumberOfSkills, viewModel.Skills.Count);
        }

        [TestMethod]
        public void AddSkill_ShowsInfoBar_WhenSkillNameExceedsMaximumLength()
        {
            int maximumSkillNameLength = 60;
            string longSkillName = new string('a', maximumSkillNameLength + 1);

            string skillNameTooLongInfoBarMessage = $"Skill name must be less than {maximumSkillNameLength} characters.";
            viewModel.AddSkill(longSkillName);

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual(skillNameTooLongInfoBarMessage, viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void RemoveSkill_RemovesSkillFromList_WhenSkillExists()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            viewModel.RemoveSkill(skill);

            int expectedNumberOfSkills = 0;
            Assert.AreEqual(expectedNumberOfSkills, viewModel.Skills.Count);
        }

        [TestMethod]
        public void AddWorkExperience_AddsExperienceToList_WhenCalled()
        {
            viewModel.AddWorkExperience();
            int expectedNumberOfWorkExperience = 1;
            Assert.AreEqual(expectedNumberOfWorkExperience, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void AddWorkExperience_DoesNotAddToList_WhenMaximumNumberOfWorkExperiencesIsReached()
        {
            int maximumNumberOfWorkExperiencesAllowed = 10;
            for (int experienceIndex = 0; experienceIndex < maximumNumberOfWorkExperiencesAllowed; experienceIndex++)
            {
                viewModel.AddWorkExperience();
            }

            viewModel.AddWorkExperience();

            Assert.AreEqual(maximumNumberOfWorkExperiencesAllowed, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void AddWorkExperience_ShowsInfoBar_WhenMaximumNumberOfWorkExperiencesIsExceeded()
        {
            int maximumNumberOfWorkExperiencesAllowed = 10;
            for (int experienceIndex = 0; experienceIndex < maximumNumberOfWorkExperiencesAllowed; experienceIndex++)
            {
                viewModel.AddWorkExperience();
            }

            viewModel.AddWorkExperience();

            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfWorkExperiencesAllowed} work experiences allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void RemoveWorkExperience_RemovesExperienceFromList_WhenExperienceExists()
        {
            viewModel.AddWorkExperience();
            var experience = viewModel.WorkExperiences.First();

            viewModel.RemoveWorkExperience(experience);
            int expectedNumberOfWorkExperience = 0;
            Assert.AreEqual(expectedNumberOfWorkExperience, viewModel.WorkExperiences.Count);
        }

        [TestMethod]
        public void AddProject_AddsProjectToList()
        {
            viewModel.AddProject();
            int expectedNumberOfProjects = 1;
            Assert.AreEqual(expectedNumberOfProjects, viewModel.Projects.Count);
        }

        [TestMethod]
        public void AddProject_DoesNotAddToList_WhenMaximumNumberOfProjectsIsReached()
        {
            int maximumNumberOfProjectsAllowed = 10;
            for (int projectIndex = 0; projectIndex < maximumNumberOfProjectsAllowed; projectIndex++)
            {
                viewModel.AddProject();
            }
            viewModel.AddProject();
            Assert.AreEqual(maximumNumberOfProjectsAllowed, viewModel.Projects.Count);
        }

        [TestMethod]
        public void AddProject_ShowsInfoBar_WhenMaximumNumberOfProjectsIsExceeded()
        {
            int maximumNumberOfProjectsAllowed = 10;
            for (int projectIndex = 0; projectIndex < maximumNumberOfProjectsAllowed; projectIndex++)
            {
                viewModel.AddProject();
            }
            viewModel.AddProject();
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfProjectsAllowed} projects allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void RemoveProject_RemovesProjectFromList_WhenProjectExists()
        {
            viewModel.AddProject();
            var project = viewModel.Projects.First();
            viewModel.RemoveProject(project);

            int expectedNumberOfProjects = 0;
            Assert.AreEqual(expectedNumberOfProjects, viewModel.Projects.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivity_AddsActivityToList()
        {
            viewModel.AddExtraCurricularActivity();
            int expectedNumberOfActivities = 1;
            Assert.AreEqual(expectedNumberOfActivities, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivity_DoesNotAddToList_WhenMaximumNumberOfActivitiesIsReached()
        {
            int maximumNumberOfExtraCurricularActivitiesAllowed = 10;
            for (int activityIndex = 0; activityIndex < maximumNumberOfExtraCurricularActivitiesAllowed; activityIndex++)
            {
                viewModel.AddExtraCurricularActivity();
            }
            viewModel.AddExtraCurricularActivity();
            Assert.AreEqual(maximumNumberOfExtraCurricularActivitiesAllowed, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void AddExtraCurricularActivity_ShowsInfoBar_WhenMaximumNumberOfActivitiesIsExceeded()
        {
            int maximumNumberOfExtraCurricularActivitiesAllowed = 10;
            for (int activityIndex = 0; activityIndex < maximumNumberOfExtraCurricularActivitiesAllowed; activityIndex++)
            {
                viewModel.AddExtraCurricularActivity();
            }
            viewModel.AddExtraCurricularActivity();
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual($"Maximum of {maximumNumberOfExtraCurricularActivitiesAllowed} extra-curricular activities allowed.", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void RemoveExtraCurricularActivity_RemovesActivityFromList_WhenActivityExists()
        {
            viewModel.AddExtraCurricularActivity();
            var activity = viewModel.ExtraCurricularActivities.First();
            viewModel.RemoveExtraCurricularActivity(activity);
            int expectedNumberOfActivities = 0;
            Assert.AreEqual(expectedNumberOfActivities, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void LoadProfile_LoadsUserProfileDataCorrectly()
        {
            UserProfile userProfile = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 25,
                University = "University of Test",
                Degree = "Bachelor's in Testing",
                ExpectedGraduationYear = 2022,
                PhoneNumber = "+40123456789",
                Skills = new List<string> { "Testing", "C#" },
                WorkExperiences = new List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "Test Company",
                        JobTitle = "Tester",
                        StartDate = new DateTime(2020, 1, 1),
                        EndDate = new DateTime(2021, 1, 1),
                        Description = "Testing software",
                        CurrentlyWorking = false
                    }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Name = "Test Project",
                        Description = "A project for testing",
                        Technologies = new List<string> { "C#, NUnit" },
                        Url = "http://testproject.com"
                    }
                },
                ExtraCurricularActivities = new List<ExtraCurricularActivity>
                {
                    new ExtraCurricularActivity
                    {
                        ActivityName = "Testing Club",
                        Description = "A club for testing enthusiasts"
                    }
                }
            };
            viewModel.LoadProfile(userProfile);
            Assert.AreEqual(userProfile.FirstName, viewModel.FirstName);
            Assert.AreEqual(userProfile.LastName, viewModel.LastName);
            Assert.AreEqual(userProfile.Skills.Count, viewModel.Skills.Count);
            Assert.AreEqual(userProfile.WorkExperiences.Count, viewModel.WorkExperiences.Count);
            Assert.AreEqual(userProfile.Projects.Count, viewModel.Projects.Count);
            Assert.AreEqual(userProfile.ExtraCurricularActivities.Count, viewModel.ExtraCurricularActivities.Count);
        }

        [TestMethod]
        public void IsDuplicateSkill_ReturnsTrueForExistingSkill()
        {
            string skill = "C#";
            viewModel.AddSkill(skill);
            Assert.IsTrue(viewModel.IsDuplicateSkill(skill));
        }

        [TestMethod]
        public void FilterSkillSuggestions_ReturnsMatchingSkill()
        {
            string searchTextQuery = "Webpa";
            List<string> results = viewModel.FilterSkillSuggestions(searchTextQuery);

            int expectedNumberOfResults = 1;
            Assert.AreEqual(expectedNumberOfResults, results.Count);
            Assert.IsTrue(results.Contains("Webpack"));
        }

        [TestMethod]
        public void FilterSkillSuggestions_ReturnsEmptyList_OnEmptyQuery()
        {
            string emptyTextQuery = string.Empty;
            List<string> results = viewModel.FilterSkillSuggestions(emptyTextQuery);
            int expectedNumberOfResults = 0;
            Assert.AreEqual(expectedNumberOfResults, results.Count);
        }

        [TestMethod]
        public void FilterSkillSuggestions_DoesntRecommendDuplicateSkill()
        {
            viewModel.AddSkill("Webpack");
            string searchTextQuery = "Webpa";
            List<string> results = viewModel.FilterSkillSuggestions(searchTextQuery);
            int expectedNumberOfResults = 0;
            Assert.AreEqual(expectedNumberOfResults, results.Count);
        }

        [TestMethod]
        public void PopulateFromParsedProfile_WorksWhenNoMissingData()
        {
            UserProfile parsedUserProfile = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 25,
                Gender = "Male",
                Email = "johndoe@gmail.com",
                Country = "Romania",
                City = "Cluj-Napoca",
                University = "University of Test",
                Degree = "Bachelor's in Testing",
                ExpectedGraduationYear = 2022,
                PhoneNumber = "+40123456789",
                Skills = new List<string> { "Testing", "C#" },
                WorkExperiences = new List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "Test Company",
                        JobTitle = "Tester",
                        StartDate = new DateTime(2020, 1, 1),
                        EndDate = new DateTime(2021, 1, 1),
                        Description = "Testing software",
                        CurrentlyWorking = false
                    }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Name = "Test Project",
                        Description = "A project for testing",
                        Technologies = new List<string> { "C#, NUnit" },
                        Url = "http://testproject.com"
                    }
                },
                ExtraCurricularActivities = new List<ExtraCurricularActivity>
                {
                    new ExtraCurricularActivity
                    {
                        ActivityName = "Testing Club",
                        Description = "A club for testing enthusiasts"
                    }
                }
            };
            viewModel.PopulateFromParsedProfile(parsedUserProfile);
            Assert.AreEqual(parsedUserProfile.FirstName, viewModel.FirstName);
            Assert.AreEqual(parsedUserProfile.LastName, viewModel.LastName);
            Assert.AreEqual(parsedUserProfile.Skills.Count, viewModel.Skills.Count);
        }

        [TestMethod]
        public void PopulateFromParsedProfile_FailsWhenMissingAge()
        {
            /// Missing mandatory fields: Age
            UserProfile parsedUserProfileMissingValues = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Email = "johndoe@gmail.com",
                Country = "Romania",
                City = "Cluj-Napoca",
                University = "University of Test",
                Degree = "Bachelor's in Testing",
                ExpectedGraduationYear = 2022,
                PhoneNumber = "+40123456789",
                Skills = new List<string> { "Testing", "C#" },
                WorkExperiences = new List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "Test Company",
                        JobTitle = "Tester",
                        StartDate = new DateTime(2020, 1, 1),
                        EndDate = new DateTime(2021, 1, 1),
                        Description = "Testing software",
                        CurrentlyWorking = false
                    }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Name = "Test Project",
                        Description = "A project for testing",
                        Technologies = new List<string> { "C#, NUnit" },
                        Url = "http://testproject.com"
                    }
                },
                ExtraCurricularActivities = new List<ExtraCurricularActivity>
                {
                    new ExtraCurricularActivity
                    {
                        ActivityName = "Testing Club",
                        Description = "A club for testing enthusiasts"
                    }
                }
            };
            viewModel.PopulateFromParsedProfile(parsedUserProfileMissingValues);
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual("Missing fields: Age", viewModel.InfoBarMessage);
        }

        [TestMethod]
        public void PopulateFromParsedProfile_FailsWhenMissingEmail()
        {
            /// Missing mandatory fields: Email
            UserProfile parsedUserProfileMissingValues = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe",
                Gender = "Male",
                Age = 25,
                Country = "Romania",
                City = "Cluj-Napoca",
                University = "University of Test",
                Degree = "Bachelor's in Testing",
                ExpectedGraduationYear = 2022,
                PhoneNumber = "+40123456789",
                Skills = new List<string> { "Testing", "C#" },
                WorkExperiences = new List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "Test Company",
                        JobTitle = "Tester",
                        StartDate = new DateTime(2020, 1, 1),
                        EndDate = new DateTime(2021, 1, 1),
                        Description = "Testing software",
                        CurrentlyWorking = false
                    }
                },
                Projects = new List<Project>
                {
                    new Project
                    {
                        Name = "Test Project",
                        Description = "A project for testing",
                        Technologies = new List<string> { "C#, NUnit" },
                        Url = "http://testproject.com"
                    }
                },
                ExtraCurricularActivities = new List<ExtraCurricularActivity>
                {
                    new ExtraCurricularActivity
                    {
                        ActivityName = "Testing Club",
                        Description = "A club for testing enthusiasts"
                    }
                }
            };
            viewModel.PopulateFromParsedProfile(parsedUserProfileMissingValues);
            Assert.IsTrue(viewModel.IsInfoBarOpen);
            Assert.AreEqual("Missing fields: Email", viewModel.InfoBarMessage);
        }
    }
}
