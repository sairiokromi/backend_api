using System.Web;

namespace Kromi.Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return properties != null ? string.Join("&", properties.ToArray()) : string.Empty;
        }
    }
}
