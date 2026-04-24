using System.Collections.Generic;

namespace PussyCatsApp.Utilities
{
    public class ProfileFormHelpers
    {
        public static bool UniversityMatchesAllWords(string university, string[] words)
        {
            foreach (string word in words)
            {
                if (!university.ToLower().Contains(word))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<string> FilterUniversitiesHelper(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<string>();
            }

            var queryWordsList = query.ToLower().Split(' ');
            var results = new List<string>();
            foreach (string university in ProfileFormData.UniversityList)
            {
                if (ProfileFormHelpers.UniversityMatchesAllWords(university, queryWordsList))
                {
                    results.Add(university);
                }
            }
            return results;
        }
    }
}
