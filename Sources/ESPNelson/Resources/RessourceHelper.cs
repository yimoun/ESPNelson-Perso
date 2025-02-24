using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ESPNelson.Resources
{
    internal class RessourceHelper
    {
        private static readonly List<string> availableLanguages = new List<string> { "fr", "en" };

        public static bool IsAvailableLanguage(string lang)
        {
            return availableLanguages.Where(l => l.Equals(lang)).FirstOrDefault() != null ? true : false;
        }

        public static string GetCurrentLanguage()
        {
            return Resource.Culture.Name;
        }

        public static string GetDefaultLanguage()
        {
            return availableLanguages[0];
        }

        public static void SetInitialLanguage()
        {
            string lang = ConfigurationManager.AppSettings["language"];

            if (!IsAvailableLanguage(lang))
                lang = GetDefaultLanguage();

            Resource.Culture = new CultureInfo(lang);
        }
    }
}
