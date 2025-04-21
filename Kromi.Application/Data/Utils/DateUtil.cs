namespace Kromi.Application.Data.Utils
{
    public static class DateUtil
    {
        private static readonly TimeZoneInfo CaracasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Caracas");
        public static DateTime Now => ConvertToCaracasTimeZone(DateTime.Now);
        public static DateTime UtcNow => ConvertToCaracasTimeZone(DateTime.UtcNow);
        public static DateTime ConvertToCaracasTimeZone(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, CaracasTimeZone);
        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static DateTime ChangeTimeUtc(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                DateTimeKind.Utc);
        }

        public static DateTime ConvertVenezuelaUtc(this DateTime date)
        {
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(date,
                       TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time")), DateTimeKind.Utc);
        }
    }
}
