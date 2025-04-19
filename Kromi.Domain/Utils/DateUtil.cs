namespace Kromi.Domain.Utils
{
    public static class DateUtil
    {
        public static DateTime ConvertVenezuelaUtc(DateTime date)
        {
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(date,
                       TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time")), DateTimeKind.Utc);
        }
    }
}
