using System;
using System.Net.Mail;

namespace Logic
{
    public static class Extensions
    {
        public static string ToMomentTime(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToString("o");
        }
        public static MailAddress GetMailAddress(this Employee e)
        {
            return new MailAddress(e.Email, e.Name + " " + e.Surname);
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalMilliseconds);
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
    }
}