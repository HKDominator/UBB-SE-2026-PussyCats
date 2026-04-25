using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models.Enumerators;

namespace PussyCatsApp.Models
{
    public class Badge
    {
        public BadgeTier Tier { get; private set; }
        public string IconPath { get; private set; }
        public int ExperiencePointsValue { get; private set; }

        public Badge(BadgeTier tier, string iconPath, int experiencePointsValue)
        {
            Tier = tier;
            IconPath = iconPath;
            ExperiencePointsValue = experiencePointsValue;
        }
    }
}
