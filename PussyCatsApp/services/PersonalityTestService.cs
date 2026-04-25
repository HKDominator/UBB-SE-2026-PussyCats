using System.Collections.Generic;
using System.Linq;
using PussyCatsApp.Factory;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
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
        private static List<Question> GetVisibilityTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("I notice design details in apps and websites that most people would overlook.", TraitType.VISIBILITY),
                QuestionFactory.Create("I believe a seamless, high-quality user interface is just as critical to a project's success as the underlying code.", TraitType.VISIBILITY),
                QuestionFactory.Create("I believe that a project isn't truly 'finished' until the visual polish matches the technical quality.", TraitType.VISIBILITY),
                QuestionFactory.Create("I find myself drawn to tools and interfaces that are clean and well-designed.", TraitType.VISIBILITY)
            ];
        }
        private static List<Question> GetInteractionTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("I enjoy collaborating with others more than working through problems on my own.", TraitType.INTERACTION),
                QuestionFactory.Create("I feel energized after meetings or group discussions rather than drained.", TraitType.INTERACTION),
                QuestionFactory.Create("I would rather manage relationships and expectations than debug a technical issue.", TraitType.INTERACTION),
                QuestionFactory.Create("I prefer roles where communication is a big part of the daily work.", TraitType.INTERACTION)
            ];
        }

        private static List<Question> GetDepthTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("When something breaks, I want to understand exactly why — not just fix the surface issue.", TraitType.DEPTH),
                QuestionFactory.Create("I enjoy reading documentation or technical material to fully understand a system.", TraitType.DEPTH),
                QuestionFactory.Create("I find it satisfying to deeply master one topic rather than know a little about many.", TraitType.DEPTH),
                QuestionFactory.Create("I get curious about what's happening \"behind the scenes\" in the tools and systems I use.", TraitType.DEPTH)
            ];
        }

        private static List<Question> GetCreativityTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("I thrive when given a problem with no clear solution rather than a checklist to follow.", TraitType.CREATIVITY),
                QuestionFactory.Create("I enjoy coming up with new ideas more than executing someone else's plan.", TraitType.CREATIVITY),
                QuestionFactory.Create("I prefer work that leaves room for experimentation over work with strict rules and procedures.", TraitType.CREATIVITY),
                QuestionFactory.Create("I am most productive when tackling new problems rather than refining existing processes.", TraitType.CREATIVITY)
            ];
        }

        private static List<Question> GetPaceTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("I work best when I have several different tasks to switch between throughout the day.", TraitType.PACE),
                QuestionFactory.Create("I enjoy fast-paced environments where priorities shift and I have to adapt quickly.", TraitType.PACE),
                QuestionFactory.Create("I prefer having many smaller responsibilities over owning one large long-term problem.", TraitType.PACE),
                QuestionFactory.Create("I feel productive when I can check off multiple different things in a single day.", TraitType.PACE)
            ];
        }

        private static List<Question> GetAbstractionTraitQuestions()
        {
            return
            [
                QuestionFactory.Create("I enjoy working with mathematical concepts, formulas, or statistical models.", TraitType.ABSTRACTION),
                QuestionFactory.Create("I find theoretical or abstract problems more interesting than purely practical ones.", TraitType.ABSTRACTION),
                QuestionFactory.Create("I am comfortable working with data, probabilities, and logical frameworks.", TraitType.ABSTRACTION),
                QuestionFactory.Create("I prefer to understand the logic and first principles of a system rather than just knowing how to operate it.", TraitType.ABSTRACTION)
            ];
        }
        public static List<Question> LoadQuestions()
        {
            QuestionFactory.ResetCounter();

            var personalityAssessmentQuestions = new List<Question>();

            personalityAssessmentQuestions.AddRange(GetVisibilityTraitQuestions());
            personalityAssessmentQuestions.AddRange(GetInteractionTraitQuestions());
            personalityAssessmentQuestions.AddRange(GetDepthTraitQuestions());
            personalityAssessmentQuestions.AddRange(GetCreativityTraitQuestions());
            personalityAssessmentQuestions.AddRange(GetPaceTraitQuestions());
            personalityAssessmentQuestions.AddRange(GetAbstractionTraitQuestions());

            return personalityAssessmentQuestions;
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

                traitScores[trait] += (int)answer.Value;
                traitQuestionCounts[trait]++;
            }

            foreach (var trait in traitScores.Keys)
            {
                traitScores[trait] /= traitQuestionCounts[trait];
            }

            return traitScores;
        }
        private double CalculateFrontend(Dictionary<TraitType, double> traitScores)
        {
            const int visibilityWeight = 2;
            const int creativiyWeight = 2;

            return (traitScores[TraitType.VISIBILITY] * visibilityWeight) +
                   (traitScores[TraitType.CREATIVITY] * creativiyWeight) +
                   traitScores[TraitType.PACE];
        }
        private double CalculateBackend(Dictionary<TraitType, double> traitScores)
        {
            const int depthWeight = 2;
            const int visibilityWeight = 2;
            const int baselineForVisibility = 5;

            return (traitScores[TraitType.DEPTH] * depthWeight) +
                   ((baselineForVisibility - traitScores[TraitType.VISIBILITY]) * visibilityWeight) +
                   traitScores[TraitType.PACE];
        }
        private double CalculateUIUX(Dictionary<TraitType, double> traitScores)
        {
            const int visibilityWeight = 3;
            const int creativityWeight = 2;

            return (traitScores[TraitType.VISIBILITY] * visibilityWeight) +
                   (traitScores[TraitType.CREATIVITY] * creativityWeight) +
                   traitScores[TraitType.INTERACTION];
        }
        private double CalculateDevOps(Dictionary<TraitType, double> traitScores)
        {
            const int depthWeight = 2;
            const int paceWeight = 2;
            const int baselineForInteraction = 5;

            return (traitScores[TraitType.DEPTH] * depthWeight) +
                   (traitScores[TraitType.PACE] * paceWeight) +
                   (baselineForInteraction - traitScores[TraitType.INTERACTION]);
        }
        private double CalculateProjectManager(Dictionary<TraitType, double> traitScores)
        {
            const int interactionWeight = 3;
            const int baselineForDepth = 5;

            return (traitScores[TraitType.INTERACTION] * interactionWeight) +
                   traitScores[TraitType.CREATIVITY] +
                   (baselineForDepth - traitScores[TraitType.DEPTH]);
        }
        private double CalculateDataAnalyst(Dictionary<TraitType, double> traitScores)
        {
            const int depthWeight = 2;
            const int abstractionWeight = 2;
            const int baselineForInteraction = 5;

            return (traitScores[TraitType.DEPTH] * depthWeight) +
                   (traitScores[TraitType.ABSTRACTION] * abstractionWeight) +
                   (baselineForInteraction - traitScores[TraitType.INTERACTION]);
        }
        private double CalculateCyberSecurity(Dictionary<TraitType, double> traitScores)
        {
            const int depthWeight = 3;
            const int baselineForInteraction = 6;
            const int baselineForPace = 6;

            return (traitScores[TraitType.DEPTH] * depthWeight) +
                   (baselineForInteraction - traitScores[TraitType.INTERACTION]) +
                   (baselineForPace - traitScores[TraitType.PACE]);
        }
        private double CalculateAIEngineer(Dictionary<TraitType, double> traitScores)
        {
            const int depthWeight = 3;
            const int abstractionWeight = 2;

            return (traitScores[TraitType.DEPTH] * depthWeight) +
                   traitScores[TraitType.CREATIVITY] +
                   (traitScores[TraitType.ABSTRACTION] * abstractionWeight);
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
        public Dictionary<JobRole, double> GetTopRoles(Dictionary<JobRole, double> roleScores, int numberOfTopRolesToReturn)
        {
            return roleScores
                .OrderByDescending(roleScorePair => roleScorePair.Value)
                .Take(numberOfTopRolesToReturn)
                .ToDictionary(roleScorePair => roleScorePair.Key, roleScorePair => roleScorePair.Value);
        }

        public void SaveResult(int userId, string personalityTestResult)
        {
            personalityTestRepository.Save(userId, personalityTestResult);
        }
    }
}
