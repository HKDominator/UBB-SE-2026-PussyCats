using System;
using System.Collections.Generic;
using PussyCatsApp.Models;
using PussyCatsApp.Repositories;
namespace PussyCatsApp.Services
{
    public class PreferenceService : IPreferenceService
    {
        private const string PreferenceTypeJobRole = "JobRole";
        private const string PreferenceTypeWorkMode = "WorkMode";
        private const string PreferenceTypeLocation = "Location";
        private const int MinRoles = 1;
        private const int MaxRoles = 3;

        private readonly IPreferenceRepository preferencesRepository;
        private List<string> predefinedLocations = new List<string>();

        public PreferenceService(IPreferenceRepository preferenceRepository)
        {
            preferencesRepository = preferenceRepository;
            LoadPredefinedLocations();
        }

        private void LoadPredefinedLocations()
        {
            predefinedLocations = new List<string>
            {
                // Romania
                "Bucharest, Romania", "Cluj-Napoca, Romania", "Timisoara, Romania", "Iasi, Romania",

                // UK & Ireland
                "London, UK", "Manchester, UK", "Birmingham, UK",
                "Dublin, Ireland",

                // Germany
                "Berlin, Germany", "Munich, Germany", "Hamburg, Germany", "Frankfurt, Germany",

                // France
                "Paris, France", "Lyon, France", "Marseille, France",

                // Spain
                "Madrid, Spain", "Barcelona, Spain", "Valencia, Spain",

                // Italy
                "Rome, Italy", "Milan, Italy", "Naples, Italy", "Florence, Italy",

                // Netherlands
                "Amsterdam, Netherlands", "Rotterdam, Netherlands", "Utrecht, Netherlands",

                // Belgium
                "Brussels, Belgium", "Antwerp, Belgium",

                // Switzerland
                "Zurich, Switzerland", "Geneva, Switzerland",

                // Austria
                "Vienna, Austria", "Salzburg, Austria",

                // Nordics
                "Stockholm, Sweden", "Gothenburg, Sweden",
                "Oslo, Norway",
                "Copenhagen, Denmark",
                "Helsinki, Finland",

                // Poland
                "Warsaw, Poland", "Krakow, Poland", "Wroclaw, Poland",

                // Czech Republic
                "Prague, Czech Republic", "Brno, Czech Republic",

                // Hungary
                "Budapest, Hungary",

                // Greece
                "Athens, Greece", "Thessaloniki, Greece",

                // Portugal
                "Lisbon, Portugal", "Porto, Portugal",

                // Balkans
                "Belgrade, Serbia",
                "Zagreb, Croatia",
                "Ljubljana, Slovenia",
                "Sarajevo, Bosnia and Herzegovina",
                "Sofia, Bulgaria",

                // Turkey
                "Istanbul, Turkey", "Ankara, Turkey",

                // USA
                "New York, USA", "Los Angeles, USA", "Chicago, USA",
                "San Francisco, USA", "Seattle, USA", "Austin, USA",
                "Boston, USA", "Miami, USA",

                // Canada
                "Toronto, Canada", "Vancouver, Canada", "Montreal, Canada",

                // Australia
                "Sydney, Australia", "Melbourne, Australia", "Brisbane, Australia",

                // Japan
                "Tokyo, Japan", "Osaka, Japan",

                // South Korea
                "Seoul, South Korea",

                // China
                "Beijing, China", "Shanghai, China", "Shenzhen, China",

                // Southeast Asia
                "Singapore, Singapore",
                "Bangkok, Thailand",
                "Kuala Lumpur, Malaysia",
                "Jakarta, Indonesia",
                "Manila, Philippines",

                // India
                "Delhi, India", "Mumbai, India", "Bangalore, India",

                // Middle East
                "Dubai, UAE", "Abu Dhabi, UAE",
                "Tel Aviv, Israel",
                "Doha, Qatar",

                // Africa
                "Cape Town, South Africa", "Johannesburg, South Africa", "Durban, South Africa",
                "Cairo, Egypt",
                "Nairobi, Kenya",
                "Lagos, Nigeria",

                // South America
                "Sao Paulo, Brazil", "Rio de Janeiro, Brazil",
                "Buenos Aires, Argentina",
                "Santiago, Chile",
                "Bogota, Colombia",
                "Lima, Peru"
            };
        }

        private bool ValidateRoleLimit(List<JobRole> roles)
        {
            if (roles == null)
            {
                return false;
            }
            return roles.Count >= MinRoles && roles.Count <= MaxRoles;
        }

        private List<Preference> BuildPreferenceRows(int userId, List<JobRole> roles, WorkMode workMode, string location)
        {
            var rows = new List<Preference>();

            foreach (var role in roles)
            {
                rows.Add(new Preference
                {
                    UserId = userId,
                    PreferenceType = PreferenceTypeJobRole,
                    Value = role.ToString()
                });
            }

            rows.Add(new Preference
            {
                UserId = userId,
                PreferenceType = PreferenceTypeWorkMode,
                Value = workMode.ToString()
            });

            rows.Add(new Preference
            {
                UserId = userId,
                PreferenceType = PreferenceTypeLocation,
                Value = location
            });

            return rows;
        }

        public List<Preference> GetByUserId(int userId)
        {
            return preferencesRepository.GetPreferencesByUserId(userId);
        }

        public void SavePreferences(int userId, List<JobRole> roles, WorkMode workMode, string location)
        {
            if (!ValidateRoleLimit(roles))
            {
                throw new ArgumentException("You must select between 1 and 3 job roles.");
            }

            var rowsToInsert = BuildPreferenceRows(userId, roles, workMode, location);

            preferencesRepository.DeleteAllByUserId(userId);

            foreach (var row in rowsToInsert)
            {
                preferencesRepository.AddPreference(row);
            }
        }
        public List<string> SearchLocations(string locationQuery)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(locationQuery))
            {
                return result;
            }

            foreach (var location in predefinedLocations)
            {
                if (location.Contains(locationQuery, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(location);
                }
            }

            return result;
        }
    }
}
