using System.Web;

namespace Kromi.Domain.Extensions
{
    public static class StringExtension
    {
        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");
            return properties != null ? string.Join("&", properties.ToArray()) : string.Empty;
        }
    }
}
