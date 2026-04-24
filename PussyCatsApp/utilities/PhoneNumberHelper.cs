using System.Linq;

namespace PussyCatsApp.Utilities
{
    public class PhoneNumberHelper
    {
        public static (string prefix, string number) ExtractPhonePrefixAndNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return (string.Empty, string.Empty);
            }

            var prefixes = new[]
            {
                "+40", "+44", "+49", "+33", "+39", "+34", "+31", "+48", "+43", "+32",
                "+46", "+351", "+420", "+36", "+359", "+30", "+45", "+358", "+353", "+385",
                "+421", "+370", "+371", "+372", "+386", "+352", "+356", "+357",
                "+1", "+61",
                "+47", "+41", "+90", "+380", "+381", "+373", "+387", "+382", "+389", "+355", "+375", "+7"
            };

            // Sort by length descending so longer prefixes match first (e.g. +351 before +3)
            foreach (var prefix in prefixes.OrderByDescending(p => p.Length))
            {
                if (phoneNumber.StartsWith(prefix))
                {
                    return (prefix, phoneNumber.Substring(prefix.Length));
                }
            }
            return (string.Empty, phoneNumber);
        }
    }
}
