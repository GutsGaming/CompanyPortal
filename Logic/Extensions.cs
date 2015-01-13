using System;
using System.Net.Mail;

namespace Logic
{
    public static class Extensions
    {
        public static string ToMomentTime(this DateTime dateTime)
        {
            return dateTime.ToString("u");
        }
        public static MailAddress GetMailAddress(this Employee e)
        {
            return new MailAddress(e.Email, e.Name + " " + e.Surname);
        }
    }
}