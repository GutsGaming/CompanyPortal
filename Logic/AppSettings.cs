using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Logic
{
    public static class AppSettings
    {
        public static SmtpClient SmtpClient
        {
            get
            {
                return new SmtpClient(Setting("SmtpHost"), Convert.ToInt32(Setting("SmtpPort")))
                {
                    EnableSsl = Convert.ToBoolean(Setting("SmtpEnableSsl")),
                    Credentials = new NetworkCredential(Setting("SmtpUsername"), Setting("SmtpPassword"))
                };
            }
        }

        public static MailAddress DefaultMailAddress
        {
            get
            {
                return new MailAddress(Setting("MailAddressFromEmail"), Setting("MailAddressFromName"));
            }
        }

        public static int StartingYear
        {
            get { return Convert.ToInt32(Setting("StartingYear")); }
        }

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