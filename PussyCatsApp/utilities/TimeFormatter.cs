using System;

namespace PussyCatsApp.Utilities
{
    public class TimeFormatter
    {
        public static string CalculateFreshnessLabel(DateTime targetDate)
        {
            TimeSpan difference = DateTime.Now.Date - targetDate.Date;
            int totalDays = (int)difference.TotalDays;

            if (totalDays == 0)
            {
                return "Profile last updated: Today";
            }
            else if (totalDays == 1)
            {
                return "Profile last updated: Yesterday";
            }
            else
            {
                return $"Profile last updated: {totalDays} days ago";
            }
        }
    }
}
