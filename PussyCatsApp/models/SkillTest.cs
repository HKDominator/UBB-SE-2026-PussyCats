using System;

namespace PussyCatsApp.Models
{
    /// <summary>
    /// Represents a skill test taken by a user, including the score,
    /// date of completion, and eligibility rules for retaking the test.
    /// </summary>
    public class SkillTest
    {
        public static readonly int MinimumScore = 0;
        public static readonly int MaximumScore = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with no score (defaults to zero).
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        public SkillTest(int skillTestId, int userId, string testName)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with a specified score.
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        /// <param name="testScore">The score achieved (0–100).</param>
        public SkillTest(int skillTestId, int userId, string testName, int testScore)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
            Score = testScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTest"/> class
        /// with a specified score and achieved date.
        /// </summary>
        /// <param name="skillTestId">The unique identifier for this skill test.</param>
        /// <param name="userId">The identifier of the user who took the test.</param>
        /// <param name="testName">The display name of the test.</param>
        /// <param name="testScore">The score achieved (0–100).</param>
        /// <param name="achievedDate">The date on which the test was completed.</param>
        public SkillTest(int skillTestId, int userId, string testName, int testScore, DateOnly achievedDate)
        {
            SkillTestId = skillTestId;
            UserId = userId;
            Name = testName;
            Score = testScore;
            AchievedDate = achievedDate;
        }

        /// <summary>
        /// Gets the unique identifier for this skill test record.
        /// </summary>
        public int SkillTestId { get; }

        /// <summary>
        /// Gets the identifier of the user who took this test.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Gets or sets the date on which this test was completed.
        /// </summary>
        public DateOnly AchievedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets or sets the name of the skill test.
        /// </summary>
        public string Name { get; set; }

        public int Score { get; set; }
    }
}
