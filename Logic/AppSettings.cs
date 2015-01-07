using System.Configuration;

namespace Logic
{
    public static class AppSettings
    {
        public static string CompanyName
        {
            get { return Setting("CompanyName"); }
        }

        public static string Setting(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}