using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;
namespace PussyCatsApp.Factory
{
    public static class QuestionFactory
    {
        private static int numberOfQuestionEntriesGenerated = 0;

        public static Question Create(string questionText, TraitType trait)
        {
            numberOfQuestionEntriesGenerated++;

            return new Question(
                numberOfQuestionEntriesGenerated,
                questionText,
                trait,
                numberOfQuestionEntriesGenerated);
        }

        public static void ResetCounter()
        {
            numberOfQuestionEntriesGenerated = 0;
        }
    }

}