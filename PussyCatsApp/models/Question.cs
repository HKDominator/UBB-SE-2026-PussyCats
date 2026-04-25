using System;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Models
{
    public class Question
    {
        public int Id { get; }
        public string QuestionText { get; }
        public TraitType Trait { get; }
        public int SortOrder { get; set; }

        public Question(int id, string questionText, TraitType trait, int sortOrder)
        {
            this.Id = id;
            this.QuestionText = questionText;
            this.Trait = trait;
            this.SortOrder = sortOrder;
        }
    }
}
