using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Utilities
{
    public class Helpers
    {
        /// <summary>
        /// Generate a random score between the specified lower and upper bounds (inclusive).
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns></returns>
        public static int GenerateRandomScore(int lowerBound, int upperBound)
        {
            Random random = new Random();
            return random.Next(lowerBound, upperBound + 1);
        }

        public static string GetFormattedNameFromJobRole(JobRole jobRole)
        {
            string formattedName = string.Empty;
            if (jobRole == JobRole.UIUXDesigner)
            {
                formattedName = "UI/UX Designer";
                return formattedName;
            }
            else if (jobRole == JobRole.AIMLEngineer)
            {
                formattedName = "AI/ML Engineer";
                return formattedName;
            }
            else
            {
                formattedName = jobRole.ToString();
            }

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (char character in formattedName)
            {
                if (char.IsUpper(character) && stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append(character);
            }
            return stringBuilder.ToString();
        }
    }
}
