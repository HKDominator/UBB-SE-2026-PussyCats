using System;

namespace PussyCatsApp.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobRole { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
    }
}
