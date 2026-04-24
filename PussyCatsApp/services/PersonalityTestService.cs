using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories.PersonalityTestRepo;
namespace PussyCatsApp.Services
{
    public class PersonalityTestService : IPersonalityTestService
    {
        private IPersonalityTestRepository personalityTestRepository;

        public PersonalityTestService(IPersonalityTestRepository personalityTestRepository)
        {
            this.personalityTestRepository = personalityTestRepository;
        }

        public static List<Question> LoadQuestions()
        {
            return
            [
                // Visibility
                new (1,  "I notice design details in apps and websites that most people would overlook.", TraitType.VISIBILITY, 1),
                new (2,  "I believe a seamless, high-quality user interface is just as critical to a project's success as the underlying code.", TraitType.VISIBILITY, 2),
                new (3,  "I believe that a project isn't truly 'finished' until the visual polish matches the technical quality.", TraitType.VISIBILITY, 3),
                new (4,  "I find myself drawn to tools and interfaces that are clean and well-designed.", TraitType.VISIBILITY, 4),

                // Interaction
                new (5,  "I enjoy collaborating with others more than working through problems on my own.", TraitType.INTERACTION, 5),
                new (6,  "I feel energized after meetings or group discussions rather than drained.", TraitType.INTERACTION, 6),
                new (7,  "I would rather manage relationships and expectations than debug a technical issue.", TraitType.INTERACTION,  7),
                new (8,  "I prefer roles where communication is a big part of the daily work.", TraitType.INTERACTION,  8),

                // Depth
                new (9,  "When something breaks, I want to understand exactly why — not just fix the surface issue.", TraitType.DEPTH, 9),
                new (10, "I enjoy reading documentation or technical material to fully understand a system.", TraitType.DEPTH, 10),
                new (11, "I find it satisfying to deeply master one topic rather than know a little about many.", TraitType.DEPTH, 11),
                new (12, "I get curious about what's happening \"behind the scenes\" in the tools and systems I use.", TraitType.DEPTH, 12),

                // Creativity
                new (13, "I thrive when given a problem with no clear solution rather than a checklist to follow.", TraitType.CREATIVITY, 13),
                new (14, "I enjoy coming up with new ideas more than executing someone else's plan.", TraitType.CREATIVITY, 14),
                new (15, "I prefer work that leaves room for experimentation over work with strict rules and procedures.", TraitType.CREATIVITY, 15),
                new (16, "I am most productive when tackling new problems rather than refining existing processes.", TraitType.CREATIVITY, 16),

                // Pace
                new (17, "I work best when I have several different tasks to switch between throughout the day.", TraitType.PACE, 17),
                new (18, "I enjoy fast-paced environments where priorities shift and I have to adapt quickly.", TraitType.PACE, 18),
                new (19, "I prefer having many smaller responsibilities over owning one large long-term problem.", TraitType.PACE, 19),
                new (20, "I feel productive when I can check off multiple different things in a single day.", TraitType.PACE, 20),

                // Abstraction
                new (21, "I enjoy working with mathematical concepts, formulas, or statistical models.", TraitType.ABSTRACTION,  21),
                new (22, "I find theoretical or abstract problems more interesting than purely practical ones.", TraitType.ABSTRACTION,  22),
                new (23, "I am comfortable working with data, probabilities, and logical frameworks.", TraitType.ABSTRACTION,  23),
                new (24, "I prefer to understand the logic and first principles of a system rather than just knowing how to operate it.", TraitType.ABSTRACTION,  24),
            ];
        }
        private int CompareRoleScores(KeyValuePair<JobRole, double> a, KeyValuePair<JobRole, double> b)
        {
            return b.Value.CompareTo(a.Value);
        }

        public Dictionary<TraitType, double> CalculateTraitScores(Dictionary<Question, AnswerValue> answers)
        {
            var traitScores = new Dictionary<TraitType, double>();
            var traitQuestionCounts = new Dictionary<TraitType, int>();

            foreach (var answer in answers)
            {
                var trait = answer.Key.Trait;
                if (!traitScores.ContainsKey(trait))
                {
                    traitScores[trait] = 0;
                    traitQuestionCounts[trait] = 0;
                }

                traitScores[trait] += answer.Value switch
                {
                    AnswerValue.STRONGLY_DISAGREE => 1,
                    AnswerValue.DISAGREE => 2,
                    AnswerValue.NEUTRAL => 3,
                    AnswerValue.AGREE => 4,
                    AnswerValue.STRONGLY_AGREE => 5
                };

                ++traitQuestionCounts[trait];
            }

            foreach (var trait in traitScores.Keys)
            {
                traitScores[trait] /= traitQuestionCounts[trait];
            }

            return traitScores;
        }
        private double CalculateFrontend(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.VISIBILITY] * 2) +
                   (traitScores[TraitType.CREATIVITY] * 2) +
                   traitScores[TraitType.PACE];
        }
        private double CalculateBackend(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.DEPTH] * 2) +
                   ((5 - traitScores[TraitType.VISIBILITY]) * 2) +
                   traitScores[TraitType.PACE];
        }
        private double CalculateUIUX(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.VISIBILITY] * 3) +
                   (traitScores[TraitType.CREATIVITY] * 2) +
                   traitScores[TraitType.INTERACTION];
        }
        private double CalculateDevOps(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.DEPTH] * 2) +
                   (traitScores[TraitType.PACE] * 2) +
                   (5 - traitScores[TraitType.INTERACTION]);
        }
        private double CalculateProjectManager(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.INTERACTION] * 3) +
                   traitScores[TraitType.CREATIVITY] +
                   (5 - traitScores[TraitType.DEPTH]);
        }
        private double CalculateDataAnalyst(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.DEPTH] * 2) +
                   (traitScores[TraitType.ABSTRACTION] * 2) +
                   (5 - traitScores[TraitType.INTERACTION]);
        }
        private double CalculateCyberSecurity(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.DEPTH] * 3) +
                   (6 - traitScores[TraitType.INTERACTION]) +
                   (6 - traitScores[TraitType.PACE]);
        }
        private double CalculateAIEngineer(Dictionary<TraitType, double> traitScores)
        {
            return (traitScores[TraitType.DEPTH] * 3) +
                   traitScores[TraitType.CREATIVITY] +
                   (traitScores[TraitType.ABSTRACTION] * 2);
        }
        public Dictionary<JobRole, double> CalculateRoleScores(Dictionary<TraitType, double> traitScores)
        {
            var roleScores = new Dictionary<JobRole, double>();

            roleScores.Add(JobRole.FrontendDeveloper, CalculateFrontend(traitScores));
            roleScores.Add(JobRole.BackendDeveloper, CalculateBackend(traitScores));
            roleScores.Add(JobRole.UIUXDesigner, CalculateUIUX(traitScores));
            roleScores.Add(JobRole.DevOpsEngineer, CalculateDevOps(traitScores));
            roleScores.Add(JobRole.ProjectManager, CalculateProjectManager(traitScores));
            roleScores.Add(JobRole.DataAnalyst, CalculateDataAnalyst(traitScores));
            roleScores.Add(JobRole.CybersecuritySpecialist, CalculateCyberSecurity(traitScores));
            roleScores.Add(JobRole.AIMLEngineer, CalculateAIEngineer(traitScores));

            return roleScores;
        }

        public Dictionary<JobRole, double> GetTopRoles(Dictionary<JobRole, double> roleScores, int length)
        {
            var list = new List<KeyValuePair<JobRole, double>>();

            foreach (var roleScorePair in roleScores)
            {
                list.Add(roleScorePair);
            }

            list.Sort(CompareRoleScores);

            var result = new Dictionary<JobRole, double>();

            int addedRoles = 0;
            foreach (var roleScorePair in list)
            {
                if (addedRoles >= length)
                {
                    break;
                }

                result.Add(roleScorePair.Key, roleScorePair.Value);
                addedRoles++;
            }

            return result;
        }
        public void SaveResult(int userId, string personalityTestResult)
        {
            personalityTestRepository.Save(userId, personalityTestResult);
        }
    }
}
